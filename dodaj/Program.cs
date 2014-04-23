using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nfield.Infrastructure;
using Nfield.Models;
using Nfield.Services;
using Ninject;

namespace dodaj
{
    class Program
    {
        static void Main(string[] args)
        {
            // Example of using the Nfield SDK with a user defined IoC container.
            // In most cases the IoC container is created and managed through the application. 
            using (IKernel kernel = new StandardKernel())
            {
                InitializeNfield(kernel);

                const string serverUrl = "http://api.nfieldbeta.com/v1";

                // First step is to get an INfieldConnection which provides services used for data access and manipulation. 
                INfieldConnection connection = NfieldConnectionFactory.Create(new Uri(serverUrl));

                // User must sign in to the Nfield server with the appropriate credentials prior to using any of the services.

                connection.SignInAsync("hendal beta", "veljko", "Klagant61").Wait();

                // Request the Interviewers service to manage interviewers.
                INfieldInterviewersService interviewersService = connection.GetService<INfieldInterviewersService>();

                // Create a new manager to perform the operations on the service.
                NfieldInterviewersManagement interviewersManager = new NfieldInterviewersManagement(interviewersService);

                // This sample shows various ways of performing synchronous and asynchronous operations on Interviewers.
                var t1 = interviewersManager.AddInterviewerAsync();
                var interviewer2 = interviewersManager.AddInterviewer();

                // Update the interviewer name asynchronously
                interviewer2.FirstName = "Harry";
                var t2 = interviewersManager.UpdateInterviewerAsync(interviewer2);

                // Wait for all pending tasks to finish
                Task.WaitAll(t1, t2);

                // Extract the results from the asynchronous tasks
                interviewer2 = t2.Result;
                var interviewer1 = t1.Result;

                // Update interviewer name synchronous
                interviewer1.EmailAddress = interviewer1.EmailAddress + "changed";
                interviewer1.FirstName = "Bob";
                interviewer1 = interviewersManager.UpdateInterviewer(interviewer1);

                // Change password for interviewer, asynchronously and synchronously
                var t3 = interviewersManager.ChangePasswordAsync(interviewer2, "ab12345");
                interviewersManager.ChangePassword(interviewer1, "12345ab");

                t3.Wait();
            }
        }

        /// <summary>
        /// Example of initializing the SDK with Ninject as the IoC container.
        /// </summary>
        private static void InitializeNfield(IKernel kernel)
        {
            DependencyResolver.Register(type => kernel.Get(type), type => kernel.GetAll(type));

            NfieldSdkInitializer.Initialize((bind, resolve) => kernel.Bind(bind).To(resolve).InTransientScope(),
                                            (bind, resolve) => kernel.Bind(bind).To(resolve).InSingletonScope(),
                                            (bind, resolve) => kernel.Bind(bind).ToConstant(resolve));
        }
    }
}
