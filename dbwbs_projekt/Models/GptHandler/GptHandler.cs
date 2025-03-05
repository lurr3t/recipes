

using Newtonsoft.Json;
using System.Net;
using System.Text;
using CsvHelper;
using dbwbs_projekt.Models.Recipes;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace dbwbs_projekt.Models.GptHandler;
public class GptHandler {
    
    private string model = OpenAI.ObjectModels.Models.Gpt_3_5_Turbo_16k;
    private int maxTokens = 10000;
    
    private OpenAIService openAiService;

    public GptHandler()
    {
        var apiKey = Environment.GetEnvironmentVariable("GPT_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new Exception("GPT_API_KEY environment variable is not set.");
        }

        openAiService = new OpenAIService(new OpenAiOptions()
        { 
            ApiKey = apiKey
        });
    }
    
    public RecipeDetails Run(string url) {

        try {
            List<string> imgUrls = new List<string>();
            
            Console.WriteLine("GptHandler run");
            string pageContent = GetTextFromUrl(url, imgUrls); 
            Task<string> jsonString = GetResultFromGpt(pageContent); 
            Console.WriteLine(jsonString.Result);
            
            RecipeDetails recipe = JsonConvert.DeserializeObject<RecipeDetails>(jsonString.Result);
            
            // Fill recipe with imgUrls
            recipe.ImgUrls = imgUrls;
            recipe.Url = url;
            QuantityFormatter(recipe);
            if (recipe != null) {
                return recipe;
            }
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }

        throw new Exception("Couldn't bind to RecipeDetails");

    }

    // Makes sure that the quantity is a number
    private void QuantityFormatter(RecipeDetails recipe) {
        if (recipe.Ingredients != null)
            foreach (IngredientDetails ingredient in recipe.Ingredients) {

                if (ingredient.IngredientQuantity != null && !int.TryParse(ingredient.IngredientQuantity, out _) &&
                    ingredient.IngredientQuantity.Contains("/")) {

                    string[] splitted =  ingredient.IngredientQuantity.Split("/");
                    // Calculates the fraction and converts it back to a string
                    ingredient.IngredientQuantity = (Convert.ToDouble(splitted[0]) / Convert.ToDouble(splitted[1])).ToString();; 

                }
                
                // should also check for ½
            }
    }
    
    // Also fills the recipeDetails object with img urls
    private string GetTextFromUrl(string url, List<string> imgUrls) {
        
        var options = new ChromeOptions();
        options.AddArgument("--headless=new");
        IWebDriver driver = new ChromeDriver(options);
        
        driver.Navigate().GoToUrl(url);
        driver.Manage().Window.Maximize();
        
        
        
        // Handle cookie popups if present
        try
        {
            // Find and click on the accept button
            IWebElement acceptButton = driver.FindElement(By.XPath("//button[contains(text(), 'Accept')]"));
            acceptButton.Click();
        }
        catch (NoSuchElementException) { }
        try
        {
            // Find and click on the accept button
            IWebElement acceptButton = driver.FindElement(By.XPath("//button[contains(text(), 'Godkänn')]"));
            acceptButton.Click();
        }
        catch (NoSuchElementException) { }
        try
        {
            // Find and click on the accept button
            IWebElement acceptButton = driver.FindElement(By.XPath("//button[contains(text(), 'Okej')]"));
            acceptButton.Click();
        }
        catch (NoSuchElementException) { }
        try
        {
            // Find and click on the accept button
            IWebElement acceptButton = driver.FindElement(By.ClassName("fc-cta-consent"));
            acceptButton.Click();
        }
        catch (NoSuchElementException) { }
        
        
        // Find all text
        IWebElement e = driver.FindElement(By.TagName("body"));
        String s = e.Text;
        
        // Find all imgUrls
      var images = driver.FindElements(By.TagName("img")); 
      
      // Add all img urls to recipeDetails and removes .svg and .gif
      foreach (var image in images) { 
          var imageUrl = image.GetAttribute("src");
          
          if (!imageUrl.EndsWith(".svg") && !imageUrl.EndsWith(".gif")) { 
              imgUrls?.Add(imageUrl); 
          } 
      }
        
        driver.Close();
        return s;
    }
    
    
    // https://github.com/betalgo/openai
    private async Task<string> GetResultFromGpt(string webText) {
        string completedText = "";

        string prePrompt =
            File.ReadAllText(
                @"/Users/ludwigfallstrom/Documents/Programmering/db_teknik/dbwbs_projekt/dbwbs_projekt/Models/GptHandler/prePromptGet.txt");
        
        openAiService.SetDefaultModelId(model);

        var completionResult = await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest {
            Messages = new List<ChatMessage> {
                ChatMessage.FromSystem(prePrompt),
                ChatMessage.FromUser(webText),
            },
            MaxTokens = maxTokens //optional
        });
        if (completionResult.Successful) {
            foreach (var choice in completionResult.Choices) {
                completedText += choice.Message.Content;
            }
        }
        else if (completionResult.Error == null) {
            throw new Exception("Unknown Error");
        }
        Console.WriteLine(completedText);
        return await Task.FromResult(completedText);
        
    }

    public async Task<List<IngredientDetails>> JoinIngredients(List<IngredientDetails> ingredients) {
        string completeInput = "";
        foreach (IngredientDetails ingredient in ingredients) {
            completeInput += ingredient.IngredientQuantity + " : " + ingredient.IngredientUnit + " : " + ingredient.IngredientName + "\n";
        }
        string prePrompt =
            File.ReadAllText(
                @"/Users/ludwigfallstrom/Documents/Programmering/db_teknik/dbwbs_projekt/dbwbs_projekt/Models/GptHandler/prePromptJoin.txt");
        
        
        //openAiService.SetDefaultModelId(OpenAI.ObjectModels.Models.Gpt_3_5_Turbo_16k);
        openAiService.SetDefaultModelId(model);
        string completedText = "";
        var completionResult = await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest {
            Messages = new List<ChatMessage> {
                ChatMessage.FromSystem(prePrompt),
                ChatMessage.FromUser(completeInput),
            },
            MaxTokens = maxTokens //optional
        });
        if (completionResult.Successful) {
            foreach (var choice in completionResult.Choices) {
                completedText += choice.Message.Content;
            }
        }
        else if (completionResult.Error == null) {
            throw new Exception("Unknown Error");
        }

        try {
            RecipeDetails recipe = JsonConvert.DeserializeObject<RecipeDetails>(completedText);
            return await Task.FromResult(recipe.Ingredients);
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
        
    } 
    
    
}