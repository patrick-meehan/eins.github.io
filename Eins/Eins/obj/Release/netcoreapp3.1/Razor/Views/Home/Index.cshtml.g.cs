#pragma checksum "C:\Users\patri\source\repos\patrick-meehan\eins.github.io\Eins\Eins\Views\Home\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "c64f22ffa7629da11e27c3858569f1cae5480074"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Index), @"mvc.1.0.view", @"/Views/Home/Index.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\patri\source\repos\patrick-meehan\eins.github.io\Eins\Eins\Views\_ViewImports.cshtml"
using Eins;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\patri\source\repos\patrick-meehan\eins.github.io\Eins\Eins\Views\_ViewImports.cshtml"
using Eins.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c64f22ffa7629da11e27c3858569f1cae5480074", @"/Views/Home/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"be60d7cf4ac04d5db6a347bbd1f344fe874b98bf", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("id", new global::Microsoft.AspNetCore.Html.HtmlString("directionarrow"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("width", new global::Microsoft.AspNetCore.Html.HtmlString("80%"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("height", new global::Microsoft.AspNetCore.Html.HtmlString("20px"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/images/dirarrow.png"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/signalr/dist/browser/signalr.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_5 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/eins.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "C:\Users\patri\source\repos\patrick-meehan\eins.github.io\Eins\Eins\Views\Home\Index.cshtml"
  
    ViewData["Title"] = "Home Page";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<div class=""container"">
    <div id=""Lobby"">
        <div class=""jumbotron-fluid"" style=""font-family:Calibri;"">
            <h1>Eins</h1>
            <p>Welcome to Eins. An oddly familiar card game built for the COVID-19 world.</p>
            
        </div>
        <div class=""card"">
            <p>To begin, enter a screen name.</p>
            <p>Have one person in your group click New Room to get a room code.</p>
            <p>Everyone else: enter the room code and join the room.</p>
            <div class=""col-2"">User</div>
            <div class=""col-4""><input type=""text"" id=""userInput"" /></div>
        </div>
        <div class=""row"">
            <div class=""col-6"">
                <input type=""button"" id=""newbutton"" value=""New Room"" />
            </div>
        </div>
        <div class=""row"">&nbsp;</div>
        <div class=""row"">&nbsp;</div>
        <div class=""row"">&nbsp;</div>
        <div class=""row"">&nbsp;</div>
        <div class=""row"">&nbsp;</div>
        <div class=");
            WriteLiteral(@"""row"">
            <div class=""col-2"">Room ID</div>
            <div class=""col-4""><input type=""text"" style=""text-transform: uppercase;"" id=""RoomID"" /></div>
        </div>
        <div class=""row"">
            <div class=""col-6"">
                <input type=""button"" id=""joinButton"" value=""Join Room"" />
            </div>
        </div>
    </div>
    <div id=""waitingroom"" style=""display: none;"">
        <div class=""row"">
            <div class=""col-8"" id=""roomcode""></div>
        </div>
        <div class=""row"">&nbsp;</div>
        <div class=""PlayerList"">
            <ol id=""Players"">
            </ol>
        </div>
        <div class=""row"">&nbsp;</div>
        <div class=""row"">
            <div class=""col-8"">
                <input type=""button"" id=""dealbutton"" value=""deal"" />
            </div>
        </div>
    </div>
    <div id=""gametable"" class=""container"" style=""display: none; background-color: white;"">
        <div class=""row"">
            <div class=""col-12"" style=""tex");
            WriteLiteral(@"t-align:center;"" id=""tableroomid""></div>
        </div>
        <div class=""row"">&nbsp;</div>
        <div class=""row"">
            <table class=""deckarea"">
                <tr>
                    <td width=""10%"">
                        <div style=""text-orientation:upright; writing-mode:vertical-lr;"" id=""leftplayer""></div>
                    </td>
                    <td width=""20%"">
                        <div class="" Card"" id=""discardPile"">&nbsp;</div>

                    </td>
                    <td width=""30%"">
                        <div><a href=""#"" class=""btn btn-info disabled endTurnBtn"" role=""button"" id=""endTurn"" onclick=""EndTurnClick()"">End Turn</a></div>
                    </td>
                    <td width=""20%"">
                        <div class=""Card CardBack"" id=""deck"">&nbsp;</div>
                    </td>
                    <td width=""10%"">
                        <div style=""text-orientation:upright; writing-mode:vertical-lr;"" id=""rightplayer""></div>
          ");
            WriteLiteral("          </td>\r\n                </tr>\r\n                <tr>\r\n                    <td width=\"10%\"></td>\r\n                    <td width=\"80%\">");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "c64f22ffa7629da11e27c3858569f1cae54800748914", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"</td>
                    <td width=""10%""></td>
                </tr>
            </table>
        </div>
        <div class=""row"">
            <div class=""col-12"">
                <hr />
            </div>
        </div>
        <div id=""MyHand"" class=""flex-container""></div>
    </div>
    <!-- The Modal -->
    <div id=""myModal"" class=""modal"">
        <!-- Modal content -->
        <div class=""modal-content"">
            <span class=""close"">&times;</span>
            <div id=""WildColorButtons"">
                <table style=""align-content: center;"">
                    <tr>
                        <td>
                            <div class=""WildColorButton"" id=""pickRed"" style=""background-color: #ff5555;"" onclick=""WildColorClick('Red')"" />
                        </td>
                        <td>
                            <div class=""WildColorButton"" id=""pickYellow"" style=""background-color: #ffaa00;"" onclick=""WildColorClick('Yellow')"" />
                        </td>
            ");
            WriteLiteral(@"        </tr>
                    <tr>
                        <td>
                            <div class=""WildColorButton"" id=""pickGreen"" style=""background-color: #55aa55;"" onclick=""WildColorClick('Green')"" />
                        </td>
                        <td>
                            <div class=""WildColorButton"" id=""pickBlue"" style=""background-color: #5555ff;"" onclick=""WildColorClick('Blue')"" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
    <div id=""Notice"" class=""modal"">
        <!-- Modal content -->
        <div class=""modal-content"">
            <div class=""col-12"" style=""text-align:center;"" id=""notification"">THis is the default</div>
        </div>
    </div>

</div>

<script src=""https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js""></script>
");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "c64f22ffa7629da11e27c3858569f1cae548007412206", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_4);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "c64f22ffa7629da11e27c3858569f1cae548007413246", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_5);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
