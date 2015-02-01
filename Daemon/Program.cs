using System;
using System.IO;
using System.Threading;
using Mono.Unix;
using log4net;
using log4net.Config;

namespace Daemon
{
    class Program
    {
        private static Thread signalThread;
        private static bool doneSignal;
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static public void Main()
        {
            // Catch SIGUSR1
            UnixSignal[] signals = new UnixSignal[] {
                new UnixSignal (Mono.Unix.Native.Signum.SIGUSR1),
                new UnixSignal (Mono.Unix.Native.Signum.SIGUSR2),
            };

            //Initialize logging configuration
            FileInfo logConfig = new FileInfo("log4net.config");
            XmlConfigurator.Configure(logConfig);

            log.Info("Daemon is starting....");

            //Setup a thread to catch the signals and start the thread running
            log.Info("Spin up signals thread.");
            StartSignal(signals);

            while (true)
            {
                //Check every second to see if we are ready to stop
                Thread.Sleep(1000);
                if (doneSignal)
                    break;
            }
            log.Info("Daemon is shutting down.");
        }

        static private void StartSignal(UnixSignal[] signals)
        {
            doneSignal = false;
            signalThread = CatchSignal(signals);
            signalThread.Start();
        }

        //Process receipt of signal
        //SIGUSR1 = shutdown daemon
        static private Thread CatchSignal(UnixSignal[] signals)
        {
            int index;
            Mono.Unix.Native.Signum signal;

            Thread signalThread = new Thread(delegate()
            {
                log.Info("Signal thread started.");

                // Wait for a signal to be delivered
                index = UnixSignal.WaitAny(signals, -1);

                // Notify the main thread that a signal was received,
                signal = signals[index].Signum;
                log.Info(String.Format("Recieved singal: {0}.", signal));
                if (signal == Mono.Unix.Native.Signum.SIGUSR1)
                {
                    log.Info("Signal thread completed.");
                    doneSignal = true;
                }
            });
            return signalThread;
        }
    }
}