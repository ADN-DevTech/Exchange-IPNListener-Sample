Copyright (c) Autodesk, Inc. All rights reserved 

Autodesk Exchange IPN Listener Sample
by Daniel Du - Autodesk Developer Network (ADN)
January 2014


Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


Exchange-IPNListener-Sample
===========================
An ASP.Net sample demonstrating how to connect and receive the Autodesk Exchange Store IPN notification.

Installation
---------------
IIS Setup:

* build the Listener solution;

* publish the website to the client box;

* Create a new Web Site in IIS called ‘IPNListener”

* Set the Physical path to IPNListener;

* In  the Application Pools, make sure IPNListener .Net framework is set to v4.0.

* Make sure all needed components (eg: ASP.NET, and mvc3) are installed in "Control Panel -> Programs and Features -> Turn Windows features on or off -> Internet Information Services".

* The server may report fail to find "default.aspx" if ASP.Net is not registered correct in IIS. Had to run the following command in the command line/run

    * 32bit (x86) Windows

       %windir%\Microsoft.NET\Framework\v4.0.30319\aspnet_regiis.exe -ir
	 
	 
    * 64bit (x64) Windows

       %windir%\Microsoft.NET\Framework64\v4.0.30319\aspnet_regiis.exe -ir
