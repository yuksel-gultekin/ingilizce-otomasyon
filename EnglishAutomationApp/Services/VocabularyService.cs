using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EnglishAutomationApp.Data;
using EnglishAutomationApp.Models;

namespace EnglishAutomationApp.Services
{
    public class VocabularyService
    {
        public static async Task<List<VocabularyWord>> GetAllWordsAsync()
        {
            using var context = new AppDbContext();
            return await context.VocabularyWords
                .Where(w => w.IsActive)
                .OrderBy(w => w.EnglishWord)
                .ToListAsync();
        }

        public static async Task<List<VocabularyWord>> GetWordsByCategoryAsync(string category)
        {
            using var context = new AppDbContext();
            return await context.VocabularyWords
                .Where(w => w.IsActive && w.Category == category)
                .OrderBy(w => w.EnglishWord)
                .ToListAsync();
        }

        public static async Task<List<VocabularyWord>> GetWordsByDifficultyAsync(WordDifficulty difficulty)
        {
            using var context = new AppDbContext();
            return await context.VocabularyWords
                .Where(w => w.IsActive && w.Difficulty == difficulty)
                .OrderBy(w => w.EnglishWord)
                .ToListAsync();
        }

        public static async Task<List<VocabularyWord>> SearchWordsAsync(string searchTerm)
        {
            using var context = new AppDbContext();
            return await context.VocabularyWords
                .Where(w => w.IsActive && 
                    (w.EnglishWord.Contains(searchTerm) || 
                     w.TurkishMeaning.Contains(searchTerm) ||
                     (w.ExampleSentence != null && w.ExampleSentence.Contains(searchTerm))))
                .OrderBy(w => w.EnglishWord)
                .ToListAsync();
        }

        public static async Task<VocabularyWord> AddWordAsync(VocabularyWord word)
        {
            using var context = new AppDbContext();
            context.VocabularyWords.Add(word);
            await context.SaveChangesAsync();
            return word;
        }

        public static async Task<VocabularyWord> UpdateWordAsync(VocabularyWord word)
        {
            using var context = new AppDbContext();
            context.VocabularyWords.Update(word);
            await context.SaveChangesAsync();
            return word;
        }

        public static async Task<bool> DeleteWordAsync(int wordId)
        {
            using var context = new AppDbContext();
            var word = await context.VocabularyWords.FindAsync(wordId);
            if (word != null)
            {
                word.IsActive = false;
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public static async Task<List<string>> GetCategoriesAsync()
        {
            using var context = new AppDbContext();
            return await context.VocabularyWords
                .Where(w => w.IsActive && !string.IsNullOrEmpty(w.Category))
                .Select(w => w.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        public static async Task<UserVocabulary?> GetUserVocabularyAsync(int userId, int wordId)
        {
            using var context = new AppDbContext();
            return await context.UserVocabularies
                .Include(uv => uv.VocabularyWord)
                .FirstOrDefaultAsync(uv => uv.UserId == userId && uv.VocabularyWordId == wordId);
        }

        public static async Task<List<UserVocabulary>> GetUserVocabulariesAsync(int userId)
        {
            using var context = new AppDbContext();
            return await context.UserVocabularies
                .Include(uv => uv.VocabularyWord)
                .Where(uv => uv.UserId == userId)
                .OrderByDescending(uv => uv.LastReviewedDate)
                .ToListAsync();
        }

        public static async Task<UserVocabulary> AddOrUpdateUserVocabularyAsync(int userId, int wordId, bool isCorrect)
        {
            using var context = new AppDbContext();
            
            var userVocab = await context.UserVocabularies
                .FirstOrDefaultAsync(uv => uv.UserId == userId && uv.VocabularyWordId == wordId);

            if (userVocab == null)
            {
                userVocab = new UserVocabulary
                {
                    UserId = userId,
                    VocabularyWordId = wordId,
                    FirstLearnedDate = DateTime.Now,
                    MasteryLevel = 1
                };
                context.UserVocabularies.Add(userVocab);
            }

            userVocab.LastReviewedDate = DateTime.Now;
            userVocab.ReviewCount++;
            userVocab.TotalAttempts++;

            if (isCorrect)
            {
                userVocab.CorrectAnswers++;
                if (userVocab.MasteryLevel < 5)
                {
                    userVocab.MasteryLevel++;
                }
            }
            else
            {
                if (userVocab.MasteryLevel > 1)
                {
                    userVocab.MasteryLevel--;
                }
            }

            await context.SaveChangesAsync();
            return userVocab;
        }

        public static async Task SeedSampleWordsAsync()
        {
            using var context = new AppDbContext();
            
            // Check if we already have words
            if (await context.VocabularyWords.AnyAsync())
                return;

            var sampleWords = new List<VocabularyWord>
            {
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
                    EnglishWord = "Sophisticated",
                    TurkishMeaning = "Sofistike, karmaşık",
                    Pronunciation = "/səˈfɪstɪkeɪtɪd/",
                    ExampleSentence = "The software has a sophisticated design.",
                    ExampleSentenceTurkish = "Yazılımın sofistike bir tasarımı var.",
                    Difficulty = WordDifficulty.Advanced,
                    PartOfSpeech = PartOfSpeech.Adjective,
                    Category = "Adjectives"
                },
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
                    EnglishWord = "Computer",
                    TurkishMeaning = "Bilgisayar",
                    Pronunciation = "/kəmˈpjuːtər/",
                    ExampleSentence = "I use my computer every day.",
                    ExampleSentenceTurkish = "Bilgisayarımı her gün kullanırım.",
                    Difficulty = WordDifficulty.Beginner,
                    PartOfSpeech = PartOfSpeech.Noun,
                    Category = "Technology"
                }
            };

            context.VocabularyWords.AddRange(sampleWords);
            await context.SaveChangesAsync();
        }
    }
}
