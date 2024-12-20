﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by Reqnroll (https://www.reqnroll.net/).
//      Reqnroll Version:1.0.0.0
//      Reqnroll Generator Version:1.0.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace MessagingService.IntegrationTests.SMS
{
    using Reqnroll;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Reqnroll", "1.0.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("SendSMS")]
    [NUnit.Framework.CategoryAttribute("base")]
    [NUnit.Framework.CategoryAttribute("shared")]
    [NUnit.Framework.CategoryAttribute("sms")]
    public partial class SendSMSFeature
    {
        
        private Reqnroll.ITestRunner testRunner;
        
        private static string[] featureTags = new string[] {
                "base",
                "shared",
                "sms"};
        
#line 1 "SendSMS.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual async System.Threading.Tasks.Task FeatureSetupAsync()
        {
            testRunner = Reqnroll.TestRunnerManager.GetTestRunnerForAssembly(null, NUnit.Framework.TestContext.CurrentContext.WorkerId);
            Reqnroll.FeatureInfo featureInfo = new Reqnroll.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "SMS", "SendSMS", null, ProgrammingLanguage.CSharp, featureTags);
            await testRunner.OnFeatureStartAsync(featureInfo);
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public virtual async System.Threading.Tasks.Task FeatureTearDownAsync()
        {
            await testRunner.OnFeatureEndAsync();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public async System.Threading.Tasks.Task TestInitializeAsync()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public async System.Threading.Tasks.Task TestTearDownAsync()
        {
            await testRunner.OnScenarioEndAsync();
        }
        
        public void ScenarioInitialize(Reqnroll.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public async System.Threading.Tasks.Task ScenarioStartAsync()
        {
            await testRunner.OnScenarioStartAsync();
        }
        
        public async System.Threading.Tasks.Task ScenarioCleanupAsync()
        {
            await testRunner.CollectScenarioErrorsAsync();
        }
        
        public virtual async System.Threading.Tasks.Task FeatureBackgroundAsync()
        {
#line 4
#line hidden
            Reqnroll.Table table6 = new Reqnroll.Table(new string[] {
                        "Name",
                        "DisplayName",
                        "Description"});
            table6.AddRow(new string[] {
                        "messagingService",
                        "Messaging REST Scope",
                        "A scope for Messaging REST"});
#line 6
 await testRunner.GivenAsync("I create the following api scopes", ((string)(null)), table6, "Given ");
#line hidden
            Reqnroll.Table table7 = new Reqnroll.Table(new string[] {
                        "ResourceName",
                        "DisplayName",
                        "Secret",
                        "Scopes",
                        "UserClaims"});
            table7.AddRow(new string[] {
                        "messagingService",
                        "Messaging REST",
                        "Secret1",
                        "messagingService",
                        ""});
#line 10
 await testRunner.GivenAsync("the following api resources exist", ((string)(null)), table7, "Given ");
#line hidden
            Reqnroll.Table table8 = new Reqnroll.Table(new string[] {
                        "ClientId",
                        "ClientName",
                        "Secret",
                        "Scopes",
                        "GrantTypes"});
            table8.AddRow(new string[] {
                        "serviceClient",
                        "Service Client",
                        "Secret1",
                        "messagingService",
                        "client_credentials"});
#line 14
 await testRunner.GivenAsync("the following clients exist", ((string)(null)), table8, "Given ");
#line hidden
            Reqnroll.Table table9 = new Reqnroll.Table(new string[] {
                        "ClientId"});
            table9.AddRow(new string[] {
                        "serviceClient"});
#line 18
 await testRunner.GivenAsync("I have a token to access the messaging service resource", ((string)(null)), table9, "Given ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Send SMS")]
        [NUnit.Framework.CategoryAttribute("PRTest")]
        public async System.Threading.Tasks.Task SendSMS()
        {
            string[] tagsOfScenario = new string[] {
                    "PRTest"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            Reqnroll.ScenarioInfo scenarioInfo = new Reqnroll.ScenarioInfo("Send SMS", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 23
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                await this.ScenarioStartAsync();
#line 4
await this.FeatureBackgroundAsync();
#line hidden
                Reqnroll.Table table10 = new Reqnroll.Table(new string[] {
                            "Sender",
                            "Destination",
                            "Message"});
                table10.AddRow(new string[] {
                            "TestSender",
                            "07777777771",
                            "TestSMSMessage"});
#line 24
 await testRunner.GivenAsync("I send the following SMS Messages", ((string)(null)), table10, "Given ");
#line hidden
            }
            await this.ScenarioCleanupAsync();
        }
    }
}
#pragma warning restore
#endregion
