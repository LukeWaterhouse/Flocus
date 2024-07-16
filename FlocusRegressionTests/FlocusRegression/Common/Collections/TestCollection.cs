using FlocusRegressionTests.Tests.Registration.AdminRegistration;
using FlocusRegressionTests.Tests.Registration.UserRegistration;
using Xunit;

namespace FlocusRegressionTests.Common.Collections;

//If things seem to go wrong CLEAN AND REBUILD
[CollectionDefinition("User Registration")]
public class UserRegistrationCollection : ICollectionFixture<UserRegistrationTestFixture>
{
}

[CollectionDefinition("Admin Registration")]
public class AdminRegistrationCollection : ICollectionFixture<AdminRegistrationTestFixture>
{
}
