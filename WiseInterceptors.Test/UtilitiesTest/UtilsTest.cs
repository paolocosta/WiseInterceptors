using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseInterceptor.Utilities;
using FluentAssertions;

namespace WiseInterceptors.Test.UtilitiesTest
{
    [TestFixture]
    public class UtilsTest
    {
        [Test]
        public void shuould_return_0_for_a_value_type()
        {
            var sut = new Utils();
            sut.GetDefaultValue(typeof(Int32)).Should().Be(0);
        }

        [Test]
        public void shuould_return_null_for_a_reference_type()
        {
            var sut = new Utils();
            sut.GetDefaultValue(typeof(System.IO.MemoryStream)).Should().Be(null);
        }
    }
}
