<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Destinatari.ascx.cs" Inherits="GestionePEC.Controls.Destinatari" %>
<%@ Register TagPrefix="uc2" TagName="Paging" Src="~/Controls/Paging.ascx" %>
<asp:UpdatePanel ID="UpdateGriglia" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="control-main" id="PanelGrid">
            <asp:GridView ID="gridDett" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                PageSize="5" DataKeyNames="Mail" OnRowCommand="Grid_RowCommand" OnRowEditing="Grid_RowEditing"
                OnDataBound="Grid_DataBound">
                <HeaderStyle CssClass="grid-header-blue" />
                <Columns>
                    <asp:TemplateField HeaderText="Destinatario" ItemStyle-HorizontalAlign="Center" HeaderStyle-Wrap="false">
                        <ItemTemplate>
                            <asp:HyperLink ToolTip="Contatto non mappato" ID="hlContact" runat="server" Visible='<%# Eval("IdContact").ToString() == "0" || Eval("RefIdReferral").ToString() == "0" %>'>
                                <%# (Eval("BackendDescr") == null || Eval("BackendDescr").ToString() == "") ? Eval("BackendCode") : Eval("BackendDescr") %>
                            </asp:HyperLink>
                            <asp:HyperLink ToolTip="Modifica contatto" ID="HyperLink1" runat="server" Visible='<%# Eval("IdContact").ToString() != "0" && Eval("RefIdReferral").ToString() != "0"%>'
                                NavigateUrl='<%# Page.ResolveClientUrl("~/crabmail/gestione-rubrica/" + (Eval("RefOrg") ?? Eval("RefIdReferral")) + "/" + Eval("IdTitolo")) %>'>
                                <%# Eval("BackendDescr") + "&nbsp;" + Eval("DescrPlus") %>
                            </asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                        <HeaderTemplate>
                            Email</HeaderTemplate>
                        <ItemTemplate>
                            <div style="border: none">
                                <uc:UCGeneralSearcher ID="GeneralSearcher1" runat="server" SearchType='<%# "ID_REFERRAL:" + (Eval("RefOrg") ?? Eval("RefIdReferral")).ToString() %>'
                                    ServicePath='<%# Page.ResolveClientUrl("~/Ashx/ListEntitaProviderGS.ashx") %>' ServiceName="FastIndexedSearch"
                                    ServiceMethod="FastSearch" DefaultSearch='<%# Eval("Mail") %>' HiddenFieldValueID='<%# Eval("IdContact") %>'
                                    ViewPortWidth="500px" />
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            Predefinito</HeaderTemplate>
                        <ItemTemplate>
                            <div style="border: none;">
                                <asp:CheckBox ID="chkDefault" runat="server" AutoPostBack="false" Visible='<%# (!String.IsNullOrEmpty(Eval("IdContact").ToString()) && (Convert.ToInt64(Eval("IdContact")) > 0))  %>' />
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <PagerTemplate>
                    <uc2:Paging ID="ucPaging" runat="server" OnPagerIndexChanged="OnPagerIndexChanged" />
                </PagerTemplate>
                <EmptyDataTemplate>
                    <label style="color: Maroon">
                        Nessun riscontro alla ricerca effettuata.
                    </label>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
