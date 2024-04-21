using System.Reflection;
using System.Runtime.Serialization;

namespace ModShardLauncherTest;

public class TableUtilsTest
{
    public class GetEnumMemberValueTest
    {
        private enum TestEnum
        {
            TestValue1,
            [EnumMember(Value = "My Value")]
            TestValue2,
            [EnumMember(Value = "")]
            TestValue3,
        }
        
        [Theory]
        [InlineData(TestEnum.TestValue1, "TestValue1")]
        [InlineData(TestEnum.TestValue2, "My Value")]
        [InlineData(TestEnum.TestValue3, "")]
        public void GetEnumMemberValue_GetProperValue(Enum value, string expectedValue)
        {
            // Arrange
            MethodInfo? methodInfo = typeof(Msl).GetMethod("GetEnumMemberValue", BindingFlags.NonPublic | BindingFlags.Static)?.MakeGenericMethod(typeof(TestEnum));
            if (methodInfo == null)
                Assert.Fail("Cannot find the tested method GetEnumMemberValue");
            
            // Act
            object? result = methodInfo.Invoke(null, new object[] { value });
            if (result == null)
                Assert.Fail("Invalid result from GetEnumMemberValue");
            
            // Assert
            Assert.Equal(expectedValue, (string)result);
        }
    }
}