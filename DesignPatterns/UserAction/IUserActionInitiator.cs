namespace KB.SharpCore.DesignPatterns.UserAction;

public interface IUserActionInitiator
{
    void OnUserActionExecuted(bool isSignificant);
}