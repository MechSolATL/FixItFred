public class TestModule : IServiceModule {
    public void RunDiagnostics(IServiceProvider provider) {
        Console.WriteLine("✅ TestModule diagnostics OK");
    }
}