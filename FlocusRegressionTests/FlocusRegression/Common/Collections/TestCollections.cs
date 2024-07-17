using FlocusRegressionTests.Tests.Registration.AdminRegistration;
using FlocusRegressionTests.Tests.Registration.UserRegistration;
using Xunit;

namespace FlocusRegressionTests.Common.Collections;

//If things seem to go wrong CLEAN AND REBUILD
[CollectionDefinition("User Registration/Deletion")]
public class UserRegistrationCollection : ICollectionFixture<UserRegistrationAndDeletionTestFixture>
{
}

[CollectionDefinition("Admin Registration/Deletion")]
public class AdminRegistrationCollection : ICollectionFixture<AdminRegistrationAndDeletionTestFixture>
{
}
