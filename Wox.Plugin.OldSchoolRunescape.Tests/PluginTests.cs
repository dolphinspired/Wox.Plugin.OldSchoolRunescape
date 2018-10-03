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

        private static List<Result> RunQuery(string search)
        {
            var query = new Query
            {
                ActionKeyword = "osrs",
                Terms = search.Split(' ')
            };

            return _program.Query(query);
        }

        [TestCase("runite ore")]
        public void TestApi(string search)
        {
            var results = RunQuery(search);

            Assert.That(results, Is.Not.Null);
            Assert.That(!results.First().Title.Contains("Error"));
        }

        [TestCase("bronze longsword")]
        public void TestBrowserStart(string search)
        {
            var results = RunQuery(search);

            results[0].Action(new ActionContext());

            // If you got here, ^that didn't blow up. gz
            Assert.That(true);
        }

        [TestCase("rune long", "rune longsword")]
        [TestCase("zulrah", "zulrah")]
        public void TestTitleMatch(string search, string expectedFirstTitle)
        {
            var results = RunQuery(search);

            Assert.That(results[0].Title.ToLower(), Is.EqualTo(expectedFirstTitle.ToLower()));
        }
    }
}
