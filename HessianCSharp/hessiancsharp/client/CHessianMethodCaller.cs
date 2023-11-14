/*
***************************************************************************************************** 
* HessianCharp - The .Net implementation of the Hessian Binary Web Service Protocol (www.caucho.com) 
* Copyright (C) 2004-2005  by D. Minich, V. Byelyenkiy, A. Voltmann
* http://www.hessiancsharp.org
*
* This library is free software; you can redistribute it and/or
* modify it under the terms of the GNU Lesser General Public
* License as published by the Free Software Foundation; either
* version 2.1 of the License, or (at your option) any later version.
*
* This library is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
* Lesser General Public License for more details.
*
* You should have received a copy of the GNU Lesser General Public
* License along with this library; if not, write to the Free Software
* Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
* 
* You can find the GNU Lesser General Public here
* http://www.gnu.org/licenses/lgpl.html
* or in the license.txt file in your source directory.
******************************************************************************************************  
* You can find all contact information on http://www.hessiancsharp.com	
******************************************************************************************************
*
*
******************************************************************************************************
* Last change: 
* 2005-08-14 Licence added  (Andre Voltmann)
* 2005-12-16 Session Cookie added (Dimitri Minich)
* 
******************************************************************************************************
*/
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Reflection;
#if COMPACT_FRAMEWORK
// do CF stuff		
#else
using System.Web;
using System.Web.SessionState;
#endif

using hessiancsharp.io;

namespace hessiancsharp.client
{
    /// <summary>
    /// Zusammenfassung f? CHessianMethodCaller.
    /// </summary>
    public class CHessianMethodCaller
    {
        #region Constants
        public const string CUSTOM_HEADER_KEY = "__CUSTOM_HEADERS";
        #endregion

        #region CLASS_FIELDS
        /// <summary>
        /// Instance of the proxy factory
        /// </summary>
        private CHessianProxyFactory m_CHessianProxyFactory;

        /// <summary>
        /// Uri for connection to the hessian service
        /// </summary>
        private Uri m_uriHessianServiceUri;
        private NetworkCredential m_credentials = null;
        #endregion

        #region PROPERTIES
        /// <summary> 
        /// Returns the connection uri to the hessian service.
        /// <value>Uri</value>
        /// </summary>
        public virtual Uri URI
        {
            get { return m_uriHessianServiceUri; }
        }
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Initializes a new instance of the CHessianMethodCaller class
        /// </summary>
        /// <param name="hessianProxyFactory">HessianProxyFactory - Instance</param>
        /// <param name="uri">Server-Proxy uri</param>
        public CHessianMethodCaller(CHessianProxyFactory hessianProxyFactory, Uri uri)
        {
            this.m_CHessianProxyFactory = hessianProxyFactory;
            this.m_uriHessianServiceUri = uri;
            this.m_credentials = (System.Net.NetworkCredential)System.Net.CredentialCache.DefaultCredentials;
        }

        public CHessianMethodCaller(CHessianProxyFactory hessianProxyFactory, Uri uri, string username, string password)
        {
            this.m_CHessianProxyFactory = hessianProxyFactory;
            this.m_uriHessianServiceUri = uri;
            this.m_credentials = new System.Net.NetworkCredential(username, password);
        }
        #endregion

        #region PUBLIC_METHODS
        /// <summary>
        /// This method wrapps an instance call to the hessian 
        /// requests, sends it to the hessian service and translates the reply of this call to the C# - data type
        /// </summary>
        /// <param name="methodInfo">The method to call</param>
        /// <param name="arrMethodArgs">The arguments to the method call</param>
        /// <returns>Invocation result</returns>
        //private WebRequest webRequest = null;
        public object DoHessianMethodCall(object[] arrMethodArgs, MethodInfo methodInfo)
        {
            Type[] argumentTypes = GetArgTypes(arrMethodArgs);
            Stream sInStream = null;
            Stream sOutStream = null;

            try
            {
                // Http Request Start
                //var startDatetime = DateTime.Now;
                //System.Diagnostics.Trace.WriteLine("[VMT RMG Http Timestamp]" + startDatetime.ToString("[HH:mm:ss:fff]") + methodInfo.ReturnParameter.Member.Name + "(+)");

                WebRequest webRequest = this.OpenConnection(m_uriHessianServiceUri);
                //if( webRequest == null)
                //    webRequest = this.OpenConnection(m_uriHessianServiceUri);
#if COMPACT_FRAMEWORK
#else
                try
                {
                    //webRequest.Headers
                    HttpWebRequest req = webRequest as HttpWebRequest;
                    //Preserve cookies to allow for session affinity between remote server and client
                    if (HttpContext.Current != null)
                    {
                        if (HttpContext.Current.Session["SessionCookie"] == null)
                        {
                            HttpContext.Current.Session.Add("SessionCookie", new CookieContainer());
                        }
                        req.CookieContainer = (CookieContainer)HttpContext.Current.Session["SessionCookie"];
                    }
                }
                catch
                {
                    //   throw e;
                    //log4net.LogManager.GetLogger(GetType()).Error("Error in setting cookie on request", e);
                }
#endif
                webRequest.Timeout = 10000; // default 100 sec --> 10 sec
                if("keepalive".Equals(methodInfo.Name.ToLower()) )
                    webRequest.Timeout = 5000;
                webRequest.ContentType = "text/xml";
                webRequest.Method = "POST";
#if COMPACT_FRAMEWORK
#else
                //Add custom headers
                if ((HttpContext.Current != null) && (HttpContext.Current.Session != null))
                {
                    AddCustomHeadersToRequest(webRequest, HttpContext.Current.Session);
                }
#endif
                MemoryStream memoryStream = new MemoryStream(2048);
                CHessianOutput cHessianOutput = this.GetHessianOutput(memoryStream);
                string strMethodName = methodInfo.Name;
                if (m_CHessianProxyFactory.IsOverloadEnabled)
                {
                    if (arrMethodArgs != null)
                    {
                        strMethodName = strMethodName + "__" + arrMethodArgs.Length;
                    }
                    else
                    {
                        strMethodName = strMethodName + "__0";
                    }
                }

                cHessianOutput.Call(strMethodName, arrMethodArgs);
                
                try
                {
                    webRequest.ContentLength = memoryStream.ToArray().Length;
                    sOutStream = webRequest.GetRequestStream();
                    memoryStream.WriteTo(sOutStream);

                    //if (false)
                    //{
                    //    var outString = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
                    //    int a = 0;
                    //}
                }
                catch (WebException wE)
                {
                    // site : http://msdn.microsoft.com/en-us/library/system.net.webexceptionstatus(v=vs.90).aspx
                    throw new CHessianException("Exception by sending request to the service with URI:\n" +
                        this.URI.ToString() + "\n" + "WebException Stauts : " + wE.Status.ToString() + "\n" + wE.Message, wE);
                }
                catch (Exception e)
                {
                    throw new CHessianException("Exception by sending request to the service with URI:\n" +
                        this.URI.ToString() + "\n" + e.Message);
                }

                sOutStream.Flush();
                sOutStream.Close();
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                if (webResponse.StatusCode != HttpStatusCode.OK)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    int chTemp;
                    sInStream = webResponse.GetResponseStream();

                    if (sInStream != null)
                    {
                        while ((chTemp = sInStream.ReadByte()) >= 0)
                        {
                            sb.Append((char)chTemp);
                        }

                        sInStream.Close();
                    }
                    throw new CHessianException(sb.ToString());
                }

                sInStream = webResponse.GetResponseStream();

                sInStream.ReadTimeout = 15000;
                sInStream.WriteTimeout = 15000;

                // Http Response End
                //var endDatetime = DateTime.Now;
                //System.Diagnostics.Trace.WriteLine("[VMT RMG Http Timestamp]" + endDatetime.ToString("[HH:mm:ss:fff]") + methodInfo.ReturnParameter.Member.Name + "(-)");
                //TimeSpan tSpan = endDatetime - startDatetime;
                //System.Diagnostics.Trace.WriteLine("[VMT RMG Http Timestamp]" + "TotalMilliseconds : " + tSpan.TotalMilliseconds.ToString());

#if COMPACT_FRAMEWORK                
				AbstractHessianInput hessianInput = this.GetHessianInput(sInStream);
#else
                System.IO.BufferedStream bStream = new BufferedStream(sInStream, 2048);
                AbstractHessianInput hessianInput = this.GetHessianInput(bStream);
                       
                //if (false)
                //{
                //    using (var reader = new StreamReader(bStream))
                //    {
                //        var readString = reader.ReadToEnd();                   
                //    }
                //}
#endif
                return hessianInput.ReadReply(methodInfo.ReturnType);
            }
            catch (Exception e)
            {
                if (e.GetType().Equals(typeof(CHessianException)))
                {
                    if ((e as CHessianException).FaultWrapper)
                        // nur ein Wrapper
                        throw e.InnerException;
                    else
                        throw e;
                }
                else
                {
                    throw new CHessianException("Exception by proxy call\n" + e.ToString() + e.Message);
                }
            }
            finally
            {
                if (sInStream != null)
                {
                    sInStream.Close();
                }
                if (sOutStream != null)
                {
                    sOutStream.Close();
                }
            }
        }

        /// <summary>
        /// Returns array with types of the instance from 
        /// the argument array
        /// </summary>
        /// <param name="arrArgs">Any array</param>
        /// <returns>Array with types of the instance from 
        /// the argument array</returns>
        public static Type[] GetArgTypes(object[] arrArgs)
        {
            if (null == arrArgs)
            {
                return new Type[0];
            }

            Type[] result = new Type[arrArgs.Length];
            for (int i = 0; i < result.Length; ++i)
            {
                if (arrArgs[i] == null)
                {
                    result[i] = null;
                }
                else
                {
                    result[i] = arrArgs[i].GetType();
                }
            }
            return result;
        }
        #endregion

        #region PRIVATE_METHODS
        /// <summary>
        /// Creates the URI connection.
        /// </summary>
        /// <param name="uri">Uri for connection</param>
        /// <returns>Request instance</returns>
        private WebRequest OpenConnection(Uri uri)
        {
            WebRequest request = WebRequest.Create(uri);
            if (this.m_credentials != null)
            {
                request.Credentials = this.m_credentials;
            }
            return request;
        }

        /// <summary>
        /// Instantiation of the hessian input (not cached) 
        /// </summary>
        /// <param name="stream">Stream for HessianInput-Instantiation</param>
        /// <returns>New HessianInput - Instance</returns>
        private AbstractHessianInput GetHessianInput(Stream stream)
        {
            return new CHessianInput(stream);
        }

        /// <summary>
        /// Instantiation of the hessian output (not cached)
        /// </summary>
        /// <param name="stream">Strean for HessianOutput - Instantiation</param>
        /// <returns>New HessianOutput - Instance</returns>
        private CHessianOutput GetHessianOutput(Stream stream)
        {
            CHessianOutput cHessianOut = new CHessianOutput(stream);
            return cHessianOut;
        }

#if COMPACT_FRAMEWORK
// do CF stuff		
#else
        private void AddCustomHeadersToRequest(WebRequest request, HttpSessionState session)
        {
            if (session[CUSTOM_HEADER_KEY] != null)
            {
                IDictionary headers = session[CUSTOM_HEADER_KEY] as IDictionary;
                foreach (DictionaryEntry entry in headers)
                {
                    request.Headers.Add("X-" + entry.Key, entry.Value.ToString());
                }
            }
        }
#endif
        #endregion
    }
}