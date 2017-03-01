using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Collections.Generic;
namespace ActiveUp.Net.Mail.DeltaExt
{

/// <summary>
/// Summary description for MailHeader
/// </summary>
public class MailHeader
{
    #region Attributes

    /// <summary>
    /// Attributes for describe a header
    /// </summary>
    public string _from;
    private string _subject;
    public DateTime _date;
    private string _index;
    private string _to;
    private string _uniqueId;
    private string _nomeFolder;

    //--//int _requestStatus=-1;
    //public int RequestStatus
    //{
    //    get { return _requestStatus; }
    //    set { _requestStatus = value; }
    //}

    #endregion

    #region Properties

    /// <summary>
    /// Represents the from header.
    /// </summary>
    public string From
    {
        get { return _from; }
        set { _from = value; }
    }

    public string NomeFolder
    {
        get { return _nomeFolder; }
        set { _nomeFolder = value; }
    }

    public string To
    {
        get { return _to; }
        set { _to = value; }
    } 

    /// <summary>
    /// Represents the Subject
    /// </summary>
    public string Subject
    {
        get { return _subject; }
        set { _subject = value; }
    }

    /// <summary>
    /// Represents the index, a identifier used for gets the Message of this header 
    /// Represent ... some sh...y things: an index and a filename ( for example ).
    /// </summary>
    public string Index
    {
        get { return _index; }
        set { _index = value; }
    }

    /// <summary>
    /// Represents the Sending Date
    /// </summary>
    public DateTime Date
    {
        get { return _date; }
        set { _date = value; }
    }

    //LR//
    public String UniqueId
    {
        get { return _uniqueId; }
        set { _uniqueId = value;}
    }

    /// <summary>
    /// Represents the Subject
    /// </summary>
	public MailHeader()
	{
        _index          = "";
        _uniqueId       = "";
    }

    #endregion

    #region Methods

    /// <summary>
    /// Fill the attributes using values passed as parameters
    /// </summary>
    /// <param name="header">The header object</param>
    public void FillHeader(string subject,string fromName,string fromEmail, System.DateTime date,string uniqueId) {
    
        this._subject = subject;
        this._uniqueId = uniqueId;
        //LR//this._from = header.From.Email;
        if ( String.IsNullOrEmpty( fromName ) )
            this._from = fromEmail;
          else
            this._from = fromName;
        this._date = date;
    }

                        
    /// <summary>
    /// Fill the attributes using the Header parameter
    /// </summary>
    /// <param name="header">The header object</param>
    public void FillHeader(Header header) {
    
        this._subject = header.Subject;
        //LR//this._from = header.From.Email;
        if ( String.IsNullOrEmpty( header.From.Name ) )
            this._from = header.From.Email;
          else
            this._from = header.From.Name;
        this._date = header.Date;
    }
  

    #endregion

    public void FillHeader(string subject,string from,DateTime date,List<string> to_emails, string name)
    {
        this._subject =subject;
        this._from = from;
        this._date = date;

        int i = 0;
        foreach (string a in to_emails)
        {
            i++;
            _to += a;
            if (to_emails.Count != i) _to += ", ";
        }


        this.Index = System.IO.Path.GetFileName(name);
    }
}

}