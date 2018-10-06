using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Wox.Plugin.OldSchoolRunescape.Tests
{
    [TestFixture]
    public class PluginTests
    {
        private static Main _program;

        [OneTimeSetUp]
        public void SetUp()
        {
            _program = new Main();
        }

        private static List<Result> RunQuery(string keyword, string search)
        {
            var query = new Query
            {
                ActionKeyword = keyword,
                Terms = search.Split(' ')
            };

            return _program.Query(query);
        }

        [TestCase("rs", "runite ore")]
        [TestCase("osrs", "runite ore")]
        public void TestApi(string keyword, string search)
        {
            var results = RunQuery(keyword, search);

            Assert.That(results, Is.Not.Null);
            Assert.That(!results.First().Title.Contains("Error"));
        }

        [TestCase("osrs", "bronze longsword")]
        public void TestBrowserStart(string keyword, string search)
        {
            var results = RunQuery(keyword, search);

            results[0].Action(new ActionContext());

            // If you got here, ^that didn't blow up. gz
            Assert.That(true);
        }

        [TestCase("rs", "rune long", "rune longsword")]
        [TestCase("osrs", "zulrah", "zulrah")]
        public void TestTitleMatch(string keyword, string search, string expectedFirstTitle)
        {
            var results = RunQuery(keyword, search);

            Assert.That(results[0].Title.ToLower(), Is.EqualTo(expectedFirstTitle.ToLower()));
        }
    }
}
