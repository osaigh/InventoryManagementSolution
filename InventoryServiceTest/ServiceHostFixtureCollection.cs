using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace InventoryServiceTest
{
    [CollectionDefinition("ServiceHostFixtureCollection")]
    public class ServiceHostFixtureCollection : ICollectionFixture<ServiceHostFixture>
    {
    }
}
