using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;

namespace ApiTests
{
    [TestFixture]
    public class BookCategoryTests : IDisposable
    {
        private RestClient client;
        private string token;

        [SetUp]
        public void Setup()
        {
            client = new RestClient(GlobalConstants.BaseUrl);
            token = GlobalConstants.AuthenticateUser("john.doe@example.com", "password123");

            Assert.That(token, Is.Not.Null.Or.Empty, "Authentication token should not be null or empty");
        }

        [Test]
        public void Test_BookCategoryLifecycle()
        {
            // Step 1: Create a new book category

            var createNewCategoryRequest = new RestRequest($"/category", Method.Post);
            createNewCategoryRequest.AddHeader("Authorization", $"Bearer {token}");

            createNewCategoryRequest.AddJsonBody(new
            {
                title = "Fictional Literature",
            });

            var createNewCategoryResponse = client.Execute(createNewCategoryRequest);

            Assert.That(createNewCategoryResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Response status code is not OK.");

            var createdCategory = JObject.Parse(createNewCategoryResponse.Content);
            var categoryId = createdCategory["_id"]?.ToString();

            Assert.That(categoryId, Is.Not.Null.Or.Empty);


            // Step 2: Retrieve all book categories

            var getAllCategoriesRequest = new RestRequest($"/category", Method.Get);
            var getAllCategoriesResponse = client.Execute(getAllCategoriesRequest);

            Assert.Multiple(() =>
            {
                Assert.That(getAllCategoriesResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Response status code is not OK.");
                Assert.That(getAllCategoriesResponse.Content, Is.Not.Null.Or.Empty, "Response content is null or empty.");

                var categories = JArray.Parse(getAllCategoriesResponse.Content);

                Assert.That(categories.Type, Is.EqualTo(JTokenType.Array), "Category type is not json array");
                Assert.That(categories.Count, Is.GreaterThan(0), "The category count is less than 1");
                Assert.That(categories.Any(c => c["_id"]?.ToString() == categoryId), Is.True);


            });

            // Step 3: Update the category title

            var updatedCategory = new { title = "Updated Fictional Literature" };
            var updateCategoryRequest = new RestRequest($"category/{categoryId}", Method.Put);
            updateCategoryRequest.AddHeader("Authorization", $"Bearer {token}");
            updateCategoryRequest.AddJsonBody(updatedCategory);

            var updateCategoryResponse = client.Execute(updateCategoryRequest);
            Assert.That(updateCategoryResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Response status code is not OK.");

            // Step 4: Verify the updated category
            var getUpdatedRequest = new RestRequest($"/category/{categoryId}", Method.Get);
            getUpdatedRequest.AddHeader("Authorization", $"Bearer {token}");

            var getUpdatedResponse = client.Execute(getUpdatedRequest);
            Assert.That(getUpdatedResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var updatedCategoryDetails = JObject.Parse(getUpdatedResponse.Content);
            Assert.Multiple(() =>
            {
                Assert.That(updatedCategoryDetails["title"]?.ToString(), Is.EqualTo(updatedCategory.title));
            });

            // Step 5: Delete the category

            var deleteCategoryRequest = new RestRequest($"/category/{categoryId}", Method.Delete);
            deleteCategoryRequest.AddHeader("Authorization", $"Bearer {token}");

            var deleteCategoryResponse = client.Execute(deleteCategoryRequest);
            Assert.That(deleteCategoryResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Response status code is not OK.");

            // Step 6: Verify the deleted category cannot be found

            var getDeletedRequest = new RestRequest($"/category/{categoryId}", Method.Get);

            var getDeletedResponse = client.Execute(getDeletedRequest);

            Assert.That(getDeletedResponse.Content, Is.EqualTo("null"), "The category content is not null.");


        }

        public void Dispose()
        {
            client?.Dispose();
        }
    }
}

/*
 using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;

namespace ApiTests
{
    [TestFixture]
    public class BookCategoryTests : IDisposable
    {
        private RestClient client;
        private string token;

        [SetUp]
        public void Setup()
        {
            client = new RestClient(GlobalConstants.BaseUrl);
            token = GlobalConstants.AuthenticateUser("john.doe@example.com", "password123");

            Assert.That(token, Is.Not.Null.Or.Empty, "Authentication token should not be null or empty");
        }

        [Test]
        public void Test_BookCategoryLifecycle()
        {
            // Step 1: Create a new book category
            var newCategory = new { title = "Fictional Literature" };
            var createCategoryRequest = new RestRequest("categories", Method.Post);
            createCategoryRequest.AddHeader("Authorization", $"Bearer {token}");
            createCategoryRequest.AddJsonBody(newCategory);

            var createCategoryResponse = client.Execute(createCategoryRequest);
            Assert.That(createCategoryResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Response status code should be 200 OK.");
            var createdCategory = JObject.Parse(createCategoryResponse.Content);
            var categoryId = createdCategory["_id"]?.ToString();
            Assert.That(categoryId, Is.Not.Null.Or.Empty, "Category ID should not be null or empty.");
            Assert.That(createdCategory["title"]?.ToString(), Is.EqualTo(newCategory.title), "Category title should match.");


            // Step 2: Retrieve all book categories and verify the newly created category is present

            var getAllCategoriesRequest = new RestRequest("categories", Method.Get);
            getAllCategoriesRequest.AddHeader("Authorization", $"Bearer {token}");
            var getAllCategoriesResponse = client.Execute(getAllCategoriesRequest);
            Assert.That(getAllCategoriesResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Response status code should be 200 OK.");
            var categories = JArray.Parse(getAllCategoriesResponse.Content);
            var retrievedCategory = categories.FirstOrDefault(c => c["_id"]?.ToString() == categoryId);
            Assert.That(retrievedCategory, Is.Not.Null, "The newly created category should appear in the list.");
            Assert.That(retrievedCategory["title"]?.ToString(), Is.EqualTo(newCategory.title), "Retrieved category title should match.");

            // Step 3: Update the category title

            var updatedCategory = new { title = "Updated Fictional Literature" };
            var updateCategoryRequest = new RestRequest($"categories/{categoryId}", Method.Put);
            updateCategoryRequest.AddHeader("Authorization", $"Bearer {token}");
            updateCategoryRequest.AddJsonBody(updatedCategory);

            var updateCategoryResponse = client.Execute(updateCategoryRequest);
            Assert.That(updateCategoryResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Response status code should be 200 OK.");


            // Step 4: Verify that the category details have been updated

            var getUpdatedCategoryRequest = new RestRequest($"categories/{categoryId}", Method.Get);
            getUpdatedCategoryRequest.AddHeader("Authorization", $"Bearer {token}");
            var getUpdatedCategoryResponse = client.Execute(getUpdatedCategoryRequest);
            Assert.That(getUpdatedCategoryResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Response status code should be 200 OK.");
            var updatedCategoryDetails = JObject.Parse(getUpdatedCategoryResponse.Content);
            Assert.That(updatedCategoryDetails["title"]?.ToString(), Is.EqualTo(updatedCategory.title), "Updated category title should match.");



            // Step 5: Delete the category and validate it's no longer accessible

            var deleteCategoryRequest = new RestRequest($"categories/{categoryId}", Method.Delete);
            deleteCategoryRequest.AddHeader("Authorization", $"Bearer {token}");
            var deleteCategoryResponse = client.Execute(deleteCategoryRequest);
            Assert.That(deleteCategoryResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Response status code should be 200 OK.");


            // Step 6: Verify that the deleted category cannot be found

            var getDeletedCategoryRequest = new RestRequest($"categories/{categoryId}", Method.Get);
            getDeletedCategoryRequest.AddHeader("Authorization", $"Bearer {token}");
            var getDeletedCategoryResponse = client.Execute(getDeletedCategoryRequest);
            Assert.That(getDeletedCategoryResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "Deleted category should not be accessible.");
            Assert.That(getDeletedCategoryResponse.Content, Is.Null.Or.Empty, "Deleted category content should be null or empty.");

        }
        [Test]
        public void Test_InvalidCategoryCreation_ShouldFail()
        {
            
            var invalidCategory = new { title = "" };
            var createCategoryRequest = new RestRequest("categories", Method.Post);
            createCategoryRequest.AddHeader("Authorization", $"Bearer {token}");
            createCategoryRequest.AddJsonBody(invalidCategory);

            
            var response = client.Execute(createCategoryRequest);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), "Expected status code to be 400 Bad Request.");
            Assert.That(response.Content, Is.Not.Null.Or.Empty, "Error response content should not be empty.");

           
            var error = JObject.Parse(response.Content);
            Assert.That(error["message"]?.ToString(), Does.Contain("title is required"), "Error message should indicate that title is required.");
        }

        [Test]
        public void Test_InvalidCategoryId_ShouldReturnNotFound()
        {
            
            var invalidId = "invalid-id";
            var getCategoryRequest = new RestRequest($"categories/{invalidId}", Method.Get);
            getCategoryRequest.AddHeader("Authorization", $"Bearer {token}");

            
            var response = client.Execute(getCategoryRequest);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "Expected status code to be 404 Not Found.");
            Assert.That(response.Content, Is.Not.Null.Or.Empty, "Error response content should not be empty.");

            
            var error = JObject.Parse(response.Content);
            Assert.That(error["message"]?.ToString(), Does.Contain("not found"), "Error message should indicate that the category was not found.");
        }

        public void Dispose()
        {
            client?.Dispose();
        }
    }
}

 */