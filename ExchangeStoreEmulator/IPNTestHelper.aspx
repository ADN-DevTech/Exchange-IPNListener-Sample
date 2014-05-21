<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IPNTestHelper.aspx.cs" Inherits="ExchangeStoreEmulator.IPNTestHelper" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Exchange Store Emulator</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        This is an simplest eumulator of Exchange Store, just to help testing your IPN 
        Listener<br />
        <br />
        <br />
        IPN Listener URL :
        <asp:TextBox ID="txtIPNListener" runat="server" Width="578px">http://localhost:48297/home/ipnlistener</asp:TextBox>
        <br />
        <br />
        -------------------------------------<br />
        <br />
        <br />
    
       AppId:&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="txtAppId" runat="server" Width="693px">appstore.exchange.autodesk.com:danielrendering:en</asp:TextBox>
        <br />
        Buyer Email:&nbsp;
        <asp:TextBox ID="txtBuyerEmail" runat="server" Width="693px">changyudu@163.com</asp:TextBox>
        <br />
        <asp:Button ID="btnBuy" runat="server" onclick="btnBuy_Click" 
            Text="Buy this app( one time payment)"/>
        <br />
        <br />
        <asp:Button ID="btnSubscribe" runat="server"  
            Text="Subscribe this app" onclick="btnSubscribe_Click" />
&nbsp;<asp:Button ID="btnRenew" runat="server" onclick="btnRenew_Click" 
            Text="Renew my subscription" />
        &nbsp;
        <asp:Button ID="btnCancel" runat="server" onclick="btnCancel_Click" 
            Text="Cancel my subscription" />
        <br />
        When customer buy an app/webservice, an IPN notification send to IPN Listener.<br />
        <br />
        <br />
        For local test/debug, set the IPN validation URL to following url in your IPN 
        Listener :
        <br />
        <asp:HyperLink ID="linkValidationIPN" runat="server">[linkValidationIPN]</asp:HyperLink>
        <br />
        -------------------------<br />
    
    </div>
    </form>
</body>
</html>
