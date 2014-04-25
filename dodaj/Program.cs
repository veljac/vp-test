using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Nfield.Infrastructure;
using Nfield.Models;
using Nfield.Services;
using Nfield.Extensions;
using Ninject;

namespace dodaj
{
    class Program
    {
        static void Main(string[] args)
        {
            using (IKernel kernel = new StandardKernel())
            {
                InitializeNfield(kernel);

                //const string serverUrl = "http://api.nfieldbeta.com/v1";
                const string serverUrl = "http://api.nfieldmr.com/v1";

                // First step is to get an INfieldConnection which provides services used for data access and manipulation. 
                INfieldConnection connection = NfieldConnectionFactory.Create(new Uri(serverUrl));

                // User must sign in to the Nfield server with the appropriate credentials prior to using any of the services.

                connection.SignInAsync("hendal", "veljko", "Klagant61").Wait();

                // Request the Interviewers service to manage dodajs.
                INfieldInterviewersService dodajsService = connection.GetService<INfieldInterviewersService>();
                INfieldFieldworkOfficesService foService = connection.GetService<INfieldFieldworkOfficesService>();
                INfieldSurveysService surveyService = connection.GetService<INfieldSurveysService>();

                // Create a new manager to perform the operations on the service.
                NfieldInterviewersManagement dodajsManager = new NfieldInterviewersManagement(dodajsService);
                NfieldSamplingPointManagement samplingPointsManager = new NfieldSamplingPointManagement(surveyService);
                NfieldFieldworkOfficesManagement foManager = new NfieldFieldworkOfficesManagement(foService);
                
                IEnumerable<Interviewer> allInterviewers = dodajsService.Query().ToList();
                var lista = allInterviewers.ToArray();

                IEnumerable<FieldworkOffice> uredi = foManager.QueryForOfficesAsync();
                var lista_ureda = uredi.ToArray();

                IEnumerable<SamplingPoint> allSamplingPoints = surveyService.SamplingPointsQuery("58e62847-57c3-4e35-ac82-5b9aba568807").ToList();
                //IEnumerable<SamplingPoint> allSamplingPoints = surveyService.SamplingPointsQuery("ce923d5d-79b6-47d2-b377-8f42174dee65").ToList();
                var allPnts = allSamplingPoints.ToArray();

                int i;

                //StreamReader spwfo = new StreamReader(@"..\..\data\sp3.txt");
                for (i=0; i<allPnts.Count(); i++)
                {
                    var tokens = spwfo.ReadLine().Split('|');
                    var mySamplingPoint = new SamplingPoint
                    {
                        SamplingPointId = tokens[0],
                        FieldworkOfficeId = tokens[1],
                        Name = tokens[2],
                        GroupId = tokens[3],
                    };
                    //samplingPointsManager.AddSamplingPoint("678726fe-5900-47a3-b1b6-7c8b2a1c7ae6", mySamplingPoint);
                    //samplingPointsManager.UpdateSamplingPoint("655059d2-0abb-4716-aaea-34549ecd6882", mySamplingPoint);
                    //Console.WriteLine(i);
                }

                SamplingPointKind spk = 0;
                var mySamplingPoint = new SamplingPoint
                {
                    SamplingPointId = "813031",
                    Description = "Survey name: EB81.3; Start address: Ulica Nikole Tesle 7, 23000, Zadar; GPS: 44.1213173,15.235273099999972",
                    Name = "Zadar - Ulica Nikole Tesle 7",
                    FieldworkOfficeId = "88a39030-c2f0-40b4-9c4e-74abe9bce433",
                    Stratum = "HR03U1",
                    GroupId = "12",
                    Kind = spk,
                };
                //samplingPointsManager.AddSamplingPoint("678726fe-5900-47a3-b1b6-7c8b2a1c7ae6", mySamplingPoint);
                //samplingPointsManager.DeleteSamplingPoint("678726fe-5900-47a3-b1b6-7c8b2a1c7ae6", mydSamplingPoint);
                //samplingPointsManager.UpdateSamplingPoint("58e62847-57c3-4e35-ac82-5b9aba568807", mySamplingPoint);
                //samplingPointsManager.UpdateSamplingPoint("655059d2-0abb-4716-aaea-34549ecd6882", mySamplingPoint);

                Console.WriteLine("Ukupno anketara: {0}", allInterviewers.Count());
                Console.WriteLine("Ukupno ureda: {0}", lista_ureda.Count());
                Console.WriteLine("Ukupno startnih točaka: {0}", allPnts.Count());

                StreamWriter datoteka = new StreamWriter(@"..\..\data\sp3.txt");
                //datoteka.WriteLine("REGIONALNI UREDI:");
                for (i = 0; i < lista_ureda.Count(); i++)
                {
                    //datoteka.WriteLine("{0}|{1}", lista_ureda[i].OfficeId, lista_ureda[i].OfficeName);
                    //Console.WriteLine("{0}|{1}", lista_ureda[i].OfficeId, lista_ureda[i].OfficeName);
                }

                //datoteka.WriteLine("STARTNE TOČKE:");
                for (i = 0; i < allSamplingPoints.Count(); i++)
                {
                    //datoteka.WriteLine("{0}|{1}|{2}|{3}|{4}|{5}|{6}", allPnts[i].SamplingPointId, allPnts[i].Description, allPnts[i].Stratum, allPnts[i].FieldworkOfficeId, allPnts[i].Name, allPnts[i].GroupId, allPnts[i].Kind);
                    Console.WriteLine("{0}|{1}|{2}|{3}|{4}", allPnts[i].SamplingPointId, allPnts[i].Name, allPnts[i].Description, allPnts[i].FieldworkOfficeId, allPnts[i].Kind);
                }


                for (i = 503; i < 882; i++)
                {
                //    IEnumerable<Interviewer> tajdodaj = dodajsService.Query().Where(dodaj => string.Equals(dodaj.UserName,"int"+i)).ToList();
                //    var talista = tajdodaj.ToArray();
                //    Interviewer mojint = talista[0];
                //    dodajsService.Remove(mojint);
                //    Console.WriteLine("{0}", "int" + i);
                }
                for (i = 5001; i < 5050; i++)
                {
                    //Console.WriteLine("{0}", "int" + i);
                    Interviewer dodaj = new Interviewer
                    {
                        UserName = "int" + i,
                        Password = "capi" + i
                    };
                    //dodajsService.Add(dodaj);
                }
            }
        }

        private static void InitializeNfield(IKernel kernel)
        {
            DependencyResolver.Register(type => kernel.Get(type), type => kernel.GetAll(type));

            NfieldSdkInitializer.Initialize((bind, resolve) => kernel.Bind(bind).To(resolve).InTransientScope(),
                                            (bind, resolve) => kernel.Bind(bind).To(resolve).InSingletonScope(),
                                            (bind, resolve) => kernel.Bind(bind).ToConstant(resolve));
        }

    }
}
