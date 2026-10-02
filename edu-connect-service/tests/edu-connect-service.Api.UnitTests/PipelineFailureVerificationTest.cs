namespace edu_connect_service.Api.UnitTests;

public class PipelineFailureVerificationTest
{
    [Fact]
    public void PipelineTest_IntentionalFailure_ShouldFail()
    {
        Assert.Fail("Fallo intencional para verificar la detencion del pipeline");
    }
}
