using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using MimeKit;
using PitStopAutoShop.Web.Data.Entities;
using PitStopAutoShop.Web.Data.Repositories;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Helpers
{
    public class MailHelper : IMailHelper
    {
        private readonly IConfiguration _configuration;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public MailHelper(IConfiguration configuration, ICustomerRepository customerRepository, IEmployeeRepository employeeRepository)
        {
            _configuration = configuration;
            _customerRepository = customerRepository;
            _employeeRepository = employeeRepository;
        }

        public List<SelectListItem> Destinations()
        {
            var options = new List<SelectListItem>();
            options.Insert(0, new SelectListItem
            {
                Text = "[Insert To Destination]",
                Value = "0"
            });
            options.Insert(1, new SelectListItem
            {
                Text = "Customers",
                Value = "1"
            });
            options.Insert(2, new SelectListItem
            {
                Text = "Employees",
                Value = "2"
            });

            return options;
        }

        public async Task<Response> SendAnnouncementAsync(int to, string subject, string body, string path)
        {            
            if(to > 0 && to < 3)
            {

                if(to == 1)
                {
                    var customers = _customerRepository.GetAll();

                    if(customers != null)
                    {
                        foreach(var customer in customers)
                        {
                            var response = await SendEmail(customer.Email, subject, body, string.IsNullOrEmpty(path)? null: path);

                            if(response.IsSuccess == false)
                            {
                                return new Response { IsSuccess = false };
                            }
                        }

                        return new Response { IsSuccess = true };
                    }
                    else
                    {
                        return new Response { IsSuccess = false };
                    }
                }

                if(to == 2)
                {
                    var employees = _employeeRepository.GetAll();

                    if (employees != null)
{
                        foreach (var employee in employees)
                        {
                            var response = await SendEmail(employee.Email, subject, body, string.IsNullOrEmpty(path) ? null : path);

                            if (response.IsSuccess == false)
                            {
                                return new Response { IsSuccess = false };
                            }
                        }                       

                        return new Response { IsSuccess = true };
                    }
                    else
                    {
                        return new Response { IsSuccess = false };
                    }
                }

                return new Response { IsSuccess = false };

            }
            else
            {
                return new Response { IsSuccess = false };
            }
        }

        public async Task<Response> SendEmail(string to, string subject, string body, string attachment)
        {
            var nameFrom = _configuration["Mail:NameFrom"];
            var from = _configuration["Mail:From"];
            var smtp = _configuration["Mail:Smtp"];
            var port = _configuration["Mail:Port"];
            var password = _configuration["Mail:Password"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(nameFrom, from));
            message.To.Add(new MailboxAddress(to, to));
            message.Subject = subject;

            var bodybuilder = new BodyBuilder
            {
                HtmlBody = body,                
            };
            
            if(attachment != null)
            {
                bodybuilder.Attachments.Add(attachment);                
            }            
            
            message.Body = bodybuilder.ToMessageBody();

            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect(smtp, int.Parse(port), false);
                    client.Authenticate(from, password);
                    await client.SendAsync(message);
                    client.Disconnect(true);
                }
            }
            catch (System.Exception ex)
            {

                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }

            return new Response
            {
                IsSuccess = true
            };
        }
    }
}
