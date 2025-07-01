using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishAutomationApp.Data;
using EnglishAutomationApp.Models;

namespace EnglishAutomationApp.Services
{
    public class VocabularyService
    {
        public static async Task<List<VocabularyWord>> GetAllWordsAsync()
        {
            return await Data.AccessDatabaseHelper.GetAllVocabularyWordsAsync();
        }

        public static async Task<List<VocabularyWord>> GetWordsByCategoryAsync(string category)
        {
            var allWords = await GetAllWordsAsync();
            return allWords.Where(w => w.Category == category).ToList();
        }

        public static async Task<List<VocabularyWord>> GetWordsByDifficultyAsync(WordDifficulty difficulty)
        {
            var allWords = await GetAllWordsAsync();
            return allWords.Where(w => w.Difficulty == difficulty).ToList();
        }

        public static async Task<List<VocabularyWord>> SearchWordsAsync(string searchTerm)
        {
            var allWords = await GetAllWordsAsync();
            return allWords.Where(w =>
                w.EnglishWord.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                w.TurkishMeaning.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (w.ExampleSentence != null && w.ExampleSentence.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        public static async Task<VocabularyWord> AddWordAsync(VocabularyWord word)
        {
            await Data.AccessDatabaseHelper.AddVocabularyWordAsync(word);
            return word;
        }

        public static async Task<VocabularyWord> UpdateWordAsync(VocabularyWord word)
        {
            // Update functionality needs to be implemented in AccessDatabaseHelper
            await Task.CompletedTask;
            return word;
        }

        public static async Task<bool> DeleteWordAsync(int wordId)
        {
            // Delete functionality needs to be implemented in AccessDatabaseHelper
            await Task.CompletedTask;
            return true;
        }

        public static async Task<List<string>> GetCategoriesAsync()
        {
            var allWords = await GetAllWordsAsync();
            return allWords
                .Where(w => !string.IsNullOrEmpty(w.Category))
                .Select(w => w.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToList();
        }

        public static async Task<UserVocabulary?> GetUserVocabularyAsync(int userId, int wordId)
        {
            // User vocabulary functionality needs to be implemented in AccessDatabaseHelper
            await Task.CompletedTask;
            return null;
        }

        public static async Task<List<UserVocabulary>> GetUserVocabulariesAsync(int userId)
        {
            return await Data.AccessDatabaseHelper.GetUserVocabularyAsync(userId);
        }

        public static async Task<UserVocabulary> AddOrUpdateUserVocabularyAsync(int userId, int wordId, bool isCorrect)
        {
            // Get existing user vocabulary or create new one
            var userVocabularyList = await Data.AccessDatabaseHelper.GetUserVocabularyAsync(userId);
            var existingUserVocab = userVocabularyList.FirstOrDefault(uv => uv.VocabularyWordId == wordId);

            if (existingUserVocab != null)
            {
                // Update existing record
                existingUserVocab.TotalAttempts++;
                if (isCorrect)
                {
                    existingUserVocab.CorrectAnswers++;
                    existingUserVocab.MasteryLevel = Math.Min(5, existingUserVocab.MasteryLevel + 1);
                }
                else
                {
                    existingUserVocab.MasteryLevel = Math.Max(1, existingUserVocab.MasteryLevel - 1);
                }
                existingUserVocab.ReviewCount++;
                existingUserVocab.LastReviewedDate = DateTime.Now;

                await Data.AccessDatabaseHelper.CreateOrUpdateUserVocabularyAsync(existingUserVocab);
                return existingUserVocab;
            }
            else
            {
                // Create new record
                var newUserVocab = new UserVocabulary
                {
                    UserId = userId,
                    VocabularyWordId = wordId,
                    FirstLearnedDate = DateTime.Now,
                    LastReviewedDate = DateTime.Now,
                    MasteryLevel = isCorrect ? 2 : 1,
                    ReviewCount = 1,
                    CorrectAnswers = isCorrect ? 1 : 0,
                    TotalAttempts = 1
                };

                await Data.AccessDatabaseHelper.CreateOrUpdateUserVocabularyAsync(newUserVocab);
                return newUserVocab;
            }
        }

        public static async Task<List<VocabularyWord>> GetWordsForReviewAsync(int userId, int maxWords = 10)
        {
            // Get random words for review - functionality needs to be implemented in AccessDatabaseHelper
            var allWords = await GetAllWordsAsync();
            return allWords.Take(maxWords).ToList();
        }

        public static async Task<Dictionary<string, int>> GetUserStatsAsync(int userId)
        {
            var userVocabulary = await Data.AccessDatabaseHelper.GetUserVocabularyAsync(userId);
            var allWords = await GetAllWordsAsync();

            var totalWordsLearned = userVocabulary.Count;
            var masteredWords = userVocabulary.Count(uv => uv.MasteryLevel >= 4);
            var wordsInProgress = userVocabulary.Count(uv => uv.MasteryLevel < 4);
            var totalReviews = userVocabulary.Sum(uv => uv.ReviewCount);
            var totalAttempts = userVocabulary.Sum(uv => uv.TotalAttempts);
            var totalCorrect = userVocabulary.Sum(uv => uv.CorrectAnswers);
            var averageAccuracy = totalAttempts > 0 ? (int)((double)totalCorrect / totalAttempts * 100) : 0;

            return new Dictionary<string, int>
            {
                ["TotalWordsLearned"] = totalWordsLearned,
                ["MasteredWords"] = masteredWords,
                ["WordsInProgress"] = wordsInProgress,
                ["TotalReviews"] = totalReviews,
                ["AverageAccuracy"] = averageAccuracy,
                ["CurrentStreak"] = 0 // This would need additional tracking
            };
        }

        public static async Task<List<VocabularyWord>> GetWeakWordsAsync(int userId, int count = 5)
        {
            var userVocabulary = await Data.AccessDatabaseHelper.GetUserVocabularyAsync(userId);
            var allWords = await GetAllWordsAsync();

            // Get words with low mastery level or low accuracy
            var weakWordIds = userVocabulary
                .Where(uv => uv.MasteryLevel <= 2 || uv.AccuracyRate < 60)
                .OrderBy(uv => uv.MasteryLevel)
                .ThenBy(uv => uv.AccuracyRate)
                .Take(count)
                .Select(uv => uv.VocabularyWordId)
                .ToList();

            return allWords.Where(w => weakWordIds.Contains(w.Id)).ToList();
        }

        private static int GetReviewInterval(int masteryLevel)
        {
            // Spaced repetition intervals in days
            return masteryLevel switch
            {
                1 => 1,  // Review after 1 day
                2 => 3,  // Review after 3 days
                3 => 7,  // Review after 1 week
                4 => 14, // Review after 2 weeks
                5 => 30, // Review after 1 month
                _ => 1
            };
        }

        private static async Task<int> CalculateCurrentStreakAsync(int userId)
        {
            // Streak calculation functionality needs to be implemented in AccessDatabaseHelper
            await Task.CompletedTask;
            return 0;
        }

        public static async Task SeedSampleWordsAsync()
        {
            // Seed data is now handled by AccessDatabaseHelper.SeedDataAsync()
            // This method is kept for backward compatibility but does nothing
            await Task.CompletedTask;
            return; // Exit early since seed data is handled by AccessDatabaseHelper

            // Legacy code below - no longer used
            var sampleWords = new List<VocabularyWord>
            {
                // Greetings
                new VocabularyWord
                {
                    EnglishWord = "Hello",
                    TurkishMeaning = "Merhaba",
                    Pronunciation = "/həˈloʊ/",
                    ExampleSentence = "Hello, how are you?",
                    ExampleSentenceTurkish = "Merhaba, nasılsın?",
                    Difficulty = WordDifficulty.Beginner,
                    PartOfSpeech = PartOfSpeech.Interjection,
                    Category = "Greetings"
                },
                new VocabularyWord
                {
                    EnglishWord = "Goodbye",
                    TurkishMeaning = "Hoşçakal, güle güle",
                    Pronunciation = "/ɡʊdˈbaɪ/",
                    ExampleSentence = "Goodbye, see you tomorrow!",
                    ExampleSentenceTurkish = "Hoşçakal, yarın görüşürüz!",
                    Difficulty = WordDifficulty.Beginner,
                    PartOfSpeech = PartOfSpeech.Interjection,
                    Category = "Greetings"
                },
                new VocabularyWord
                {
                    EnglishWord = "Thank you",
                    TurkishMeaning = "Teşekkür ederim",
                    Pronunciation = "/θæŋk juː/",
                    ExampleSentence = "Thank you for your help.",
                    ExampleSentenceTurkish = "Yardımın için teşekkür ederim.",
                    Difficulty = WordDifficulty.Beginner,
                    PartOfSpeech = PartOfSpeech.Interjection,
                    Category = "Greetings"
                },

                // Family
                new VocabularyWord
                {
                    EnglishWord = "Family",
                    TurkishMeaning = "Aile",
                    Pronunciation = "/ˈfæməli/",
                    ExampleSentence = "I love spending time with my family.",
                    ExampleSentenceTurkish = "Ailemle vakit geçirmeyi seviyorum.",
                    Difficulty = WordDifficulty.Beginner,
                    PartOfSpeech = PartOfSpeech.Noun,
                    Category = "Family"
                },
                new VocabularyWord
                {
                    EnglishWord = "Mother",
                    TurkishMeaning = "Anne",
                    Pronunciation = "/ˈmʌðər/",
                    ExampleSentence = "My mother is a teacher.",
                    ExampleSentenceTurkish = "Annem bir öğretmen.",
                    Difficulty = WordDifficulty.Beginner,
                    PartOfSpeech = PartOfSpeech.Noun,
                    Category = "Family"
                },
                new VocabularyWord
                {
                    EnglishWord = "Father",
                    TurkishMeaning = "Baba",
                    Pronunciation = "/ˈfɑːðər/",
                    ExampleSentence = "My father works in a bank.",
                    ExampleSentenceTurkish = "Babam bir bankada çalışıyor.",
                    Difficulty = WordDifficulty.Beginner,
                    PartOfSpeech = PartOfSpeech.Noun,
                    Category = "Family"
                },

                // Adjectives
                new VocabularyWord
                {
                    EnglishWord = "Beautiful",
                    TurkishMeaning = "Güzel",
                    Pronunciation = "/ˈbjuːtɪfəl/",
                    ExampleSentence = "She has a beautiful smile.",
                    ExampleSentenceTurkish = "Onun güzel bir gülümsemesi var.",
                    Difficulty = WordDifficulty.Beginner,
                    PartOfSpeech = PartOfSpeech.Adjective,
                    Category = "Adjectives"
                },
                new VocabularyWord
                {
                    EnglishWord = "Intelligent",
                    TurkishMeaning = "Zeki, akıllı",
                    Pronunciation = "/ɪnˈtelɪdʒənt/",
                    ExampleSentence = "She is a very intelligent student.",
                    ExampleSentenceTurkish = "O çok zeki bir öğrenci.",
                    Difficulty = WordDifficulty.Intermediate,
                    PartOfSpeech = PartOfSpeech.Adjective,
                    Category = "Adjectives"
                },
                new VocabularyWord
                {
                    EnglishWord = "Sophisticated",
                    TurkishMeaning = "Sofistike, karmaşık",
                    Pronunciation = "/səˈfɪstɪkeɪtɪd/",
                    ExampleSentence = "The software has a sophisticated design.",
                    ExampleSentenceTurkish = "Yazılımın sofistike bir tasarımı var.",
                    Difficulty = WordDifficulty.Advanced,
                    PartOfSpeech = PartOfSpeech.Adjective,
                    Category = "Adjectives"
                },

                // Education
                new VocabularyWord
                {
                    EnglishWord = "Learn",
                    TurkishMeaning = "Öğrenmek",
                    Pronunciation = "/lɜːrn/",
                    ExampleSentence = "I want to learn English.",
                    ExampleSentenceTurkish = "İngilizce öğrenmek istiyorum.",
                    Difficulty = WordDifficulty.Beginner,
                    PartOfSpeech = PartOfSpeech.Verb,
                    Category = "Education"
                },
                new VocabularyWord
                {
                    EnglishWord = "Study",
                    TurkishMeaning = "Çalışmak, ders çalışmak",
                    Pronunciation = "/ˈstʌdi/",
                    ExampleSentence = "I study English every day.",
                    ExampleSentenceTurkish = "Her gün İngilizce çalışıyorum.",
                    Difficulty = WordDifficulty.Beginner,
                    PartOfSpeech = PartOfSpeech.Verb,
                    Category = "Education"
                },
                new VocabularyWord
                {
                    EnglishWord = "Knowledge",
                    TurkishMeaning = "Bilgi",
                    Pronunciation = "/ˈnɒlɪdʒ/",
                    ExampleSentence = "Knowledge is power.",
                    ExampleSentenceTurkish = "Bilgi güçtür.",
                    Difficulty = WordDifficulty.Intermediate,
                    PartOfSpeech = PartOfSpeech.Noun,
                    Category = "Education"
                },

                // Technology
                new VocabularyWord
                {
                    EnglishWord = "Computer",
                    TurkishMeaning = "Bilgisayar",
                    Pronunciation = "/kəmˈpjuːtər/",
                    ExampleSentence = "I use my computer every day.",
                    ExampleSentenceTurkish = "Bilgisayarımı her gün kullanırım.",
                    Difficulty = WordDifficulty.Beginner,
                    PartOfSpeech = PartOfSpeech.Noun,
                    Category = "Technology"
                },
                new VocabularyWord
                {
                    EnglishWord = "Internet",
                    TurkishMeaning = "İnternet",
                    Pronunciation = "/ˈɪntərnet/",
                    ExampleSentence = "The internet connects people worldwide.",
                    ExampleSentenceTurkish = "İnternet dünya çapında insanları bağlar.",
                    Difficulty = WordDifficulty.Beginner,
                    PartOfSpeech = PartOfSpeech.Noun,
                    Category = "Technology"
                },
                new VocabularyWord
                {
                    EnglishWord = "Artificial Intelligence",
                    TurkishMeaning = "Yapay Zeka",
                    Pronunciation = "/ˌɑːrtɪˈfɪʃəl ɪnˈtelɪdʒəns/",
                    ExampleSentence = "Artificial intelligence is changing our world.",
                    ExampleSentenceTurkish = "Yapay zeka dünyamızı değiştiriyor.",
                    Difficulty = WordDifficulty.Advanced,
                    PartOfSpeech = PartOfSpeech.Noun,
                    Category = "Technology"
                }
            };

            // Add sample words to database
            foreach (var word in sampleWords)
            {
                word.CreatedDate = DateTime.Now;
                await Data.AccessDatabaseHelper.AddVocabularyWordAsync(word);
            }
        }
    }
}
