﻿[assembly: System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
[assembly: Xunit.AssemblyTrait("Type", "Integration")]
[assembly: Xunit.CollectionBehavior(DisableTestParallelization = true)]
[assembly: Xunit.TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
[assembly: Xunit.TestCollectionOrderer("Xunit.Extensions.Ordering.CollectionOrderer", "Xunit.Extensions.Ordering")]
