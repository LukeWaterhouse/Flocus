using FlocusRegressionTests.Tests._02_GetUser._02._01_UserGetUser;
using FlocusRegressionTests.Tests._02_GetUser._02._02_AdminGetUser;
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

[CollectionDefinition("User GetUser")]
public class UserGetUserCollection : ICollectionFixture<UserGetUserTestFixture>
{
}

[CollectionDefinition("Admin GetUser")]
public class AdminGetUserCollection : ICollectionFixture<AdminGetUserTestFixture>
{
}
