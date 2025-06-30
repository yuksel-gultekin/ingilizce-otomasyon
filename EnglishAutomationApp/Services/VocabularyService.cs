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
            return await AccessDatabaseHelper.GetAllVocabularyWordsAsync();
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
            await AccessDatabaseHelper.AddVocabularyWordAsync(word);
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
            // User vocabulary functionality needs to be implemented in AccessDatabaseHelper
            await Task.CompletedTask;
            return new List<UserVocabulary>();
        }

        public static async Task<UserVocabulary> AddOrUpdateUserVocabularyAsync(int userId, int wordId, bool isCorrect)
        {
            // User vocabulary functionality needs to be implemented in AccessDatabaseHelper
            await Task.CompletedTask;
            return new UserVocabulary
            {
                UserId = userId,
                VocabularyWordId = wordId,
                FirstLearnedDate = DateTime.Now,
                MasteryLevel = 1
            };
        }

        public static async Task<List<VocabularyWord>> GetWordsForReviewAsync(int userId, int maxWords = 10)
        {
            // Get random words for review - functionality needs to be implemented in AccessDatabaseHelper
            var allWords = await GetAllWordsAsync();
            return allWords.Take(maxWords).ToList();
        }

        public static async Task<Dictionary<string, int>> GetUserStatsAsync(int userId)
        {
            // User stats functionality needs to be implemented in AccessDatabaseHelper
            await Task.CompletedTask;
            var allWords = await GetAllWordsAsync();
            return new Dictionary<string, int>
            {
                ["TotalWordsLearned"] = allWords.Count,
                ["MasteredWords"] = 0,
                ["WordsInProgress"] = allWords.Count,
                ["TotalReviews"] = 0,
                ["AverageAccuracy"] = 0,
                ["CurrentStreak"] = 0
            };
        }

        public static async Task<List<VocabularyWord>> GetWeakWordsAsync(int userId, int count = 5)
        {
            // Weak words functionality needs to be implemented in AccessDatabaseHelper
            var allWords = await GetAllWordsAsync();
            return allWords.Take(count).ToList();
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
            // Check if we already have words
            var existingWords = await GetAllWordsAsync();
            if (existingWords.Any())
                return;

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
                await AccessDatabaseHelper.AddVocabularyWordAsync(word);
            }
        }
    }
}
