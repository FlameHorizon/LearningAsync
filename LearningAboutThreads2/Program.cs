namespace LearningAboutThreads2;

// ReSharper disable once ClassNeverInstantiated.Global
public class Example
{
    public static void Main()
    {
        // StartingThreadWithoutArgs();
        // StartingThreadWithArgs();
        // RetrievingThreadInformation();
        // ForegroundAndBackgroundThreads();
        HowMyProgramMightLookLike();
    }

    private static void HowMyProgramMightLookLike()
    {
        CancellationTokenSource cts = new();
        CancellationToken token = cts.Token;
        Task task = Task.Run(() => ObserveWindows(token), token);
        
        Console.WriteLine($"Task Id {task.Id}");
        
        var codes = new List<string> { "A", "B", "C" };
        foreach (string code in codes)
        {
            Thread.Sleep(1000);
            Console.WriteLine($"Main thread: Processing code: {code}");

            if (code == "B")
            {
                cts.Cancel();
                //Console.WriteLine($"Was told to cancel {token.IsCancellationRequested}");
            }
        }
        cts.Dispose();
    }

    private static void ObserveWindows(CancellationToken token)
    {
        while (token.IsCancellationRequested == false)
        {
            Thread.Sleep(1000);
            Console.WriteLine("WindowObserver: Checking for new windows");
        }

        Console.WriteLine("WindowObserver: Exiting");
    }

    private static void ForegroundAndBackgroundThreads()
    {
        // Instances of the Thread class represent either
        // foreground threads or background threads.
        // Background threads are identical to foreground threads
        // with one exception: a background thread does not keep a process
        // running if all foreground threads have terminated.
        // Once all foreground threads have been stopped,
        // the runtime stops all background threads and shuts down.

        // Task create a background thread which stops when main thread stops.
        var t = new Thread(new ThreadStart(BackgroundThreadWorker));
        t.IsBackground = true;

        t.Start();
        t.Interrupt();
        Thread.Sleep(1000);
        Console.WriteLine("Main thread: Exiting program...");
    }

    private static void BackgroundThreadWorker()
    {
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine($"Background thread: {i}");
            Thread.Sleep(300);
        }
    }

    private static Object _lock = new();

    private static void RetrievingThreadInformation()
    {
        ThreadPool.QueueUserWorkItem(ShowThreadInformation);
        var th1 = new Thread(ShowThreadInformation);
        th1.Start();

        var th2 = new Thread(ShowThreadInformation);
        th2.IsBackground = true;
        th2.Start();

        var th3 = Thread.CurrentThread;
        th3.Name = "Main Thread";
        ShowThreadInformation(null);
    }

    // Caller QueueUserWorkItem expected ShowThreadInformation to be void and to
    // have arguments of type object.
    private static void ShowThreadInformation(object? obj)
    {
        // When I remove the lock, the output is not in the order I expect.
        // ShowThreadInformation is called by multiple threads, 
        // and order of messages is not guaranteed.
        lock (_lock)
        {
            Console.WriteLine($"Thread ID: {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine($"Thread Name: {Thread.CurrentThread.Name}");
            Console.WriteLine($"Thread Priority: {Thread.CurrentThread.Priority}");
            Console.WriteLine($"Thread State: {Thread.CurrentThread.ThreadState}");
            Console.WriteLine($"Thread IsBackground: {Thread.CurrentThread.IsBackground}");
            Console.WriteLine($"Thread IsThreadPoolThread: {Thread.CurrentThread.IsThreadPoolThread}");
            Console.WriteLine($"Thread ApartmentState: {Thread.CurrentThread.GetApartmentState()}");
        }
    }

    private static void StartingThreadWithArgs()
    {
        Console.WriteLine("Main thread: Start a second thread with arguments");

        var t = new Thread(ThreadProcArgs);

        t.Start(10);
        t.Join();
    }

    private static void ThreadProcArgs(object? o)
    {
        if (o == null) throw new ArgumentNullException(nameof(o));
        var startCountFrom = (int)o;

        for (int i = startCountFrom; i < startCountFrom + 4; i++)
        {
            Console.WriteLine("ThreadProc: {0}", i);
            Thread.Sleep(100);
        }
    }

    private static void StartingThreadWithoutArgs()
    {
        Console.WriteLine("Main thread: Start a second thread.");

        var t = new Thread(new ThreadStart(ThreadProc));
        t.Start();

        for (int i = 0; i < 4; i++)
        {
            Console.WriteLine($"Main thread: Do some work {i}.");
            Thread.Sleep(150);
        }

        Console.WriteLine("Main thread: Call Join(), to wait until ThreadProc ends.");
        t.Join();
        Console.WriteLine("Main thread: ThreadProc.Join has returned.  Press Enter to end program.");
        Console.ReadLine();
    }

    private static void ThreadProc()
    {
        for (int i = 0; i < 2; i++)
        {
            Console.WriteLine("ThreadProc: {0}", i);
            Thread.Sleep(100);
        }
    }
}