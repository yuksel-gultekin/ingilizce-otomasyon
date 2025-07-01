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
            await Data.AccessDatabaseHelper.UpdateVocabularyWordAsync(word);
            return word;
        }

        public static async Task<bool> DeleteWordAsync(int wordId)
        {
            return await Data.AccessDatabaseHelper.DeleteVocabularyWordAsync(wordId);
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
            try
            {
                var allWords = await GetAllWordsAsync();
                var userVocabulary = await GetUserVocabulariesAsync(userId);

                // Get words that need review based on spaced repetition
                var wordsNeedingReview = new List<VocabularyWord>();

                foreach (var word in allWords)
                {
                    var userVocab = userVocabulary.FirstOrDefault(uv => uv.VocabularyWordId == word.Id);

                    if (userVocab == null)
                    {
                        // New word - add to review
                        wordsNeedingReview.Add(word);
                    }
                    else if (userVocab.LastReviewedDate.HasValue)
                    {
                        // Check if enough time has passed for review
                        var daysSinceReview = (DateTime.Now - userVocab.LastReviewedDate.Value).Days;
                        var reviewInterval = GetReviewInterval(userVocab.MasteryLevel);

                        if (daysSinceReview >= reviewInterval)
                        {
                            wordsNeedingReview.Add(word);
                        }
                    }
                    else
                    {
                        // Never reviewed - add to review
                        wordsNeedingReview.Add(word);
                    }
                }

                // Prioritize words with lower mastery levels and shuffle
                var prioritizedWords = wordsNeedingReview
                    .OrderBy(w => userVocabulary.FirstOrDefault(uv => uv.VocabularyWordId == w.Id)?.MasteryLevel ?? 0)
                    .ThenBy(w => Guid.NewGuid()) // Shuffle within same mastery level
                    .Take(maxWords)
                    .ToList();

                return prioritizedWords;
            }
            catch (Exception)
            {
                // Fallback to random words if there's an error
                var allWords = await GetAllWordsAsync();
                return allWords.OrderBy(w => Guid.NewGuid()).Take(maxWords).ToList();
            }
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
        }
    }
}
