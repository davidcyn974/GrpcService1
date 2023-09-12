using Grpc.Core;
using GrpcService1;
using MimeKit.Text;
using MimeKit;
using GrpcService1.Resources;
using DotLiquid;
using Org.BouncyCastle.Asn1.Ocsp;
using static System.Net.Mime.MediaTypeNames;

namespace GrpcService1.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        #region Private Readonly Fields
        private readonly ILogger<GreeterService> _logger;

        private readonly IConfiguration _configuration;

        private readonly IEmailService _emailService;

        #endregion

        public GreeterService(ILogger<GreeterService> logger, IConfiguration configuration, IEmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
            _logger = logger;
        }

        

        public override async Task ServerStreaming(Test request, IServerStreamWriter<Test> responseStream, ServerCallContext context)
        {
            for (int i = 0; i <= 10; i++)
            {
                await responseStream.WriteAsync(new Test { TestMessage = "i = " + i });
                await Task.Delay(1000);
            }
        }


        public override async Task<Test> ClientStreaming(IAsyncStreamReader<Test> requestStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext(CancellationToken.None))
            {
                Console.WriteLine(requestStream.Current.TestMessage); // prints out each client's message
                await Task.Delay(1000);
            }
            return new Test { TestMessage = "Server's job done."  };
        }


        public override async Task BidirectionalStreaming(IAsyncStreamReader<Test> requestStream, IServerStreamWriter<Test> responseStream, ServerCallContext context)
        {
            List<Task> tasks = new List<Task>();
            while( await requestStream.MoveNext(CancellationToken.None))
            {
                var message = requestStream.Current.TestMessage;
                Console.WriteLine("Received request = " + message );
                Task task = Task.Run(async () =>
                {
                    Random random = new Random();
                    await Task.Delay( random.Next(1,10) * 1000 );
                    await responseStream.WriteAsync(new Test { TestMessage = "Server sent : " + message });
                }
                );
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }


        // Say hello & Send mail methods
        //public override async Task<TiensJeTeDonneReply> SayHello(DonneMoiRequest request, ServerCallContext context)
        //await sendRefusCollecteMailAsync(_configuration, _emailService, new List<string> { "david.chane@ioconnect.re" }, "0", "0", "0", " 0");
        public override Task<TiensJeTeDonneReply> SayHello(DonneMoiRequest request, ServerCallContext context)

        {
            return Task.FromResult(new TiensJeTeDonneReply
            {
                Message = "Voici " + request.Name + ", il a " + request.Age + " ans. Son sexe est de type :" + request.Sex
            });
        }

        public static async Task sendRefusCollecteMailAsync(IConfiguration configuration, IEmailService mailService, List<string> mailTo, string numeroCollecte, string adherentLibelle, string pdcLibelle, string motifRefus)
        {
            var countMail = 0;
            var templateToRender = Template.Parse(ResourcesManager.TemplateRefusCollecte);
            var m = new MimeMessage();
            m.From.Add(MailboxAddress.Parse(configuration["MailServer:From"]));
            mailTo.ForEach(s =>
            {
                if (!string.IsNullOrEmpty(s))
                {
                    m.To.Add(MailboxAddress.Parse(s));
                    countMail++;
                }
            });
            //m.Bcc.Add(MailboxAddress.Parse("valentin.senardiere@ioconnect.re")); countMail++; //TODO remove
            m.Subject = "test subject";
            m.Priority = MessagePriority.Urgent;
            m.Body = new TextPart(TextFormat.Html)
            {
                Text = templateToRender.Render(
                    Hash.FromAnonymousObject(
                        new
                        {
                            numeroCollecte = numeroCollecte,
                            adherentLibelle = adherentLibelle,
                            pdcLibelle = pdcLibelle,
                            motifRefus = motifRefus
                        }
                    )
                )
            };
            if (countMail > 0)
            {
                await mailService.SendCollecteMailAsync(m);
            }
        }

    }


}

