using System;
using System.Collections.Generic;

namespace Redmine8842.UnitTests
{
    internal sealed class TestRunner
    {
        private readonly List<TestCase> tests = new List<TestCase>();

        public void Add(string name, Action test)
        {
            tests.Add(new TestCase(name, test));
        }

        public int RunAll()
        {
            int passed = 0;
            int failed = 0;

            foreach (TestCase test in tests)
            {
                try
                {
                    test.Action();
                    passed++;
                    Console.WriteLine("[PASS] " + test.Name);
                }
                catch (Exception ex)
                {
                    failed++;
                    Console.WriteLine("[FAIL] " + test.Name);
                    Console.WriteLine("       " + ex.GetType().Name + ": " + ex.Message);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Passed: " + passed);
            Console.WriteLine("Failed: " + failed);

            return failed == 0 ? 0 : 1;
        }

        private sealed class TestCase
        {
            public TestCase(string name, Action action)
            {
                Name = name;
                Action = action;
            }

            public string Name { get; private set; }
            public Action Action { get; private set; }
        }
    }
}