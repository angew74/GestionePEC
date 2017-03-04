<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MailComposer.ascx.cs" Inherits="GestionePEC.Controls.MailComposer" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="ActiveUp.Net.Mail" %>
<%@ Import Namespace="ActiveUp.Net.Common.DeltaExt" %>
<%@ Register Assembly="GestionePEC" Namespace="GestionePEC.Extensions" TagPrefix="inl" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<div class="content-panel" style="height: 500px; overflow: auto;">
    <div class="header-panel-gray">
        <div class="header-title">
            <div class="header-text-left">
                <label>
                    Nuovo Messaggio</label>
            </div>
            <div class="header-text-right">
                <asp:LinkButton ID="Button1" runat="server" Text="Invia" OnClick="Button1_Click"
                    CausesValidation="false"></asp:LinkButton>
            </div>
        </div>
    </div>
    <div class="body-panel-gray">
        <asp:Panel runat="server" ID="pnlMail">
            <div style="display: table; width: 90%;">
                <div style="display: table-row;">
                    <div style="display: table-cell;">
                        <label>
                            Da:</label>
                    </div>
                    <div style="display: table-cell; width: 100%;">
                        <label>
                            <%# String.IsNullOrEmpty(CurrentMessage.From.Name) ? CurrentMessage.From.Email : CurrentMessage.From.Name %></label>
                    </div>
                </div>
                <div style="display: table-row;">
                    <div style="display: table-cell;">
                        <label>
                            A:</label>
                    </div>
                    <div style="display: table-cell; width: 100%;">
                        <asp:TextBox ID="ToTextBox" runat="server" Width="100%" Text='<%# String.Join(";", CurrentMessage.To.Select(x => x.Email).ToArray()) %>' />
                    </div>
                </div>
                <div style="display: table-row;">
                    <div style="display: table-cell;">
                        <label>
                            Cc:</label>
                    </div>
                    <div style="display: table-cell;">
                        <asp:TextBox ID="CCTextBox" runat="server" Width="100%" Text='<%# String.Join(";", CurrentMessage.Cc.Select(x => x.Email).ToArray()) %>' />
                    </div>
                </div>
                <div style="display: table-row;">
                    <div style="display: table-cell;">
                        <label>
                            Ccn:</label>
                    </div>
                    <div style="display: table-cell;">
                        <asp:TextBox ID="BCCTextBox" runat="server" Width="100%" Text='<%# String.Join(";", CurrentMessage.Bcc.Select(x => x.Email).ToArray()) %>' />
                    </div>
                </div>
                <div style="display: table-row;">
                    <div style="display: table-cell;">
                        <label>
                            Oggetto:</label>
                    </div>
                    <div style="display: table-cell;">
                        <asp:TextBox ID="SubjectTextBox" runat="server" Width="100%" Text='<%# CurrentMessage.Subject %>' />
                    </div>
                </div>
                <div runat="server" style="display: table-row;" visible='<%# (EnableNewAttachments == true) && DivsVisibility[Divs.NewAttachments] %>'>
                    <div style="display: table-cell;">
                        <label>
                            Nuovi allegati:</label>
                    </div>
                    <div style="display: table-cell;">
                        <inl:InlineScript ID="InlineScript1" runat="server">

                            <script type="text/javascript">
                                var uploadError = false;
                                function onUploadError(){ uploadError = true; }
                                function onUploadComplete(){                                   
                                    uploadError = false; 
                                }
                                function validateForm(){ return !uploadError; }
                            </script>

                        </inl:InlineScript>
                        <cc1:AsyncFileUpload runat="server" ID="asyncFileUpload" ThrobberID="throbber" Width="500px"
                            UploaderStyle="Traditional" OnClientUploadError="onUploadError" OnClientUploadComplete="onUploadComplete"
                            OnUploadedComplete="asyncFileUpload_UploadedComplete" OnUploadedFileError="asyncFileUpload_UploadedFileError" />
                        <asp:Image runat="server" ID="throbber" ImageUrl="~/App_Themes/Delta/images/tree/loading.gif" />
                        <asp:Button ID="Button2" runat="server" Text="Aggiungi" OnClick="btnPostFileUpload_Click" />
                        <asp:Repeater runat="server" ID="rptNewAttach" DataSource='<%# CurrentMessage.Attachments.Cast<MimePart>().Where(x => x.ParentMessage == null) %>'
                            OnItemCommand="rpAttach_ItemCommand" Visible='<%# CurrentMessage.Attachments.Cast<MimePart>().Where(x => x.ParentMessage == null).Count() != 0 %>'>
                            <ItemTemplate>
                                <div>
                                    <asp:ImageButton runat="server" ID="ibRemoveNewAttach" ImageAlign="Middle" ImageUrl="~/App_Themes/Delta/images/buttons/delete.png"
                                        CommandName="RemoveAttach" CommandArgument='<%# Eval("Filename") %>' />
                                    <label>
                                        <%# Eval("Filename") %>;</label>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
                <% if (EnableAttachments)
                   { %>
                <div style="display: table-row;">
                    <div style="display: table-cell; vertical-align: text-top;">
                        <label>
                            Allegati:</label>
                    </div>
                    <div style="display: table-cell;">
                        <div>
                            <asp:CheckBox runat="server" ID="cbIncludiAllegati" Text="Includi allegati originali"
                                Font-Italic="true" Checked='<%# DivsVisibility[Divs.Attachments] %>' Visible='<%# IncludiAllegatiVisible() %>'
                                ToolTip="Clic per includere gli allegati" AutoPostBack="true" OnCheckedChanged="cbIncludiAllegati_CheckedChanged" />
                        </div>
                        <asp:Repeater runat="server" ID="rpAttach" DataSource='<%# CurrentMessage.Attachments.Cast<MimePart>().Where(x => x.ParentMessage != null) %>'
                            OnItemCommand="rpAttach_ItemCommand" Visible='<%# DivsVisibility[Divs.Attachments] %>'>
                            <ItemTemplate>
                                <div>
                                    <asp:ImageButton runat="server" ID="ibRemoveAttach" ImageAlign="Middle" ImageUrl="~/App_Themes/Delta/images/buttons/delete.png"
                                        CommandName="RemoveAttach" CommandArgument='<%# Eval("Filename") %>' Visible='<%# MailEditabile %>' />
                                    <label>
                                        <%# Eval("Filename") %>;</label>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
                <%} %>
            </div>
            <div style="display: table; width: 90%">
                <div style="display: table-row;">
                    <div style="display: table-cell; width: 100%; padding: 10px; max-height: 300px; overflow: auto;">
                        <asp:TextBox ID="BodyTextBox" runat="server" Height="112px" Width="100%" TextMode="MultiLine" />
                        <asp:Literal ID="PreformattedBody" runat="server" Text='<%# ParseOriginalMessage() %>' />
                    </div>
                </div>
                <div style="display: table-row; padding: 10px;">
                    <div style="display: table-cell;">
                        <asp:Label ID="ErrorLabel" runat="server" ForeColor="Red" Text="Label"></asp:Label>
                        <asp:Label ID="Label5" runat="server" Font-Size="Medium" Text="Il messaggio è stato spedito correttamente"></asp:Label>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</div>
