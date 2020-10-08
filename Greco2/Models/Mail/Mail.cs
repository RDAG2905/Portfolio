using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Greco2.Models.Mail
{
    public class Mail
    {
     
      
            string From = ""; 
            string To;  
            string Message;  
            string Subject;
            List<string> Archivo = new List<string>(); 
            
            System.Net.Mail.MailMessage Email;

            public string error = "";

            
            public Mail(string FROM, string Para, string Mensaje, string Asunto, List<string> ArchivoPedido_ = null)
            {
                From = FROM;
                To = Para;
                Message = Mensaje;
                Subject = Asunto;
                Archivo = ArchivoPedido_;

            }

           
            public bool enviaMail()
            {
            
                if (To.Trim().Equals("") || Message.Trim().Equals("") || Subject.Trim().Equals(""))
                {
                    error = "El mail, el asunto y el mensaje son obligatorios";
                    return false;
                }

               
                //comienza-------------------------------------------------------------------------
                try
                {
                    
                    Email = new System.Net.Mail.MailMessage(From, To, Subject, Message);

                    
                    if (Archivo != null)
                    {
                        
                        foreach (string archivo in Archivo)
                        {
                            
                            if (System.IO.File.Exists(@archivo))
                                Email.Attachments.Add(new Attachment(@archivo));

                        }
                    }

                Email.IsBodyHtml = true;
                Email.From = new MailAddress(From); 

                    
                    //System.Net.Mail.SmtpClient smtpMail = new System.Net.Mail.SmtpClient("smtp.gmail.com");
                    System.Net.Mail.SmtpClient smtpMail = new System.Net.Mail.SmtpClient("SMTPAPPL01.telecom.com.ar");
                    smtpMail.EnableSsl = false;//le definimos si es conexión ssl
                    smtpMail.UseDefaultCredentials = false; //le decimos que no utilice la credencial por defecto
                                                            //smtpMail.Host = "smtp.gmail.com"; //agregamos el servidor smtp
                                                            //smtpMail.Port = 587; //le asignamos el puerto, en este caso gmail utiliza el 465
                    smtpMail.Host = "SMTPAPPL01.telecom.com.ar"; 
                    smtpMail.Port = 25; 
                    //smtpMail.Credentials = new System.Net.NetworkCredential(DE, PASS); 

                   
                    smtpMail.Send(Email);

                   
                    smtpMail.Dispose();

                    
                    return true;
                }
                catch (Exception ex)
                {
                   
                    error = "Ocurrio un error: " + ex.Message;
                    return false;
                }

                

            }
        
    }
}
