using FlocusRegressionTests.Tests.Registration.UserRegistration;
using Xunit;

namespace FlocusRegressionTests.Common.Collections;

//If things seem to go wrong CLEAN AND REBUILD
[CollectionDefinition("User Registration")]
public class TestCollection : ICollectionFixture<UserRegistrationTestFixture>
{
}
