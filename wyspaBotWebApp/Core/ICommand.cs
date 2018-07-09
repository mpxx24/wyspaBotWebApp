using System.Collections.Generic;

namespace wyspaBotWebApp.Core {
    public interface ICommand {
        IEnumerable<string> GetText();
    }

    public interface ICommandWithStringParameter {
        IEnumerable<string> GetText(string parameter);
    }

    public interface ICommandWithStringIenumerableParameter {
        IEnumerable<string> GetText(IEnumerable<string> parameter);
    }

    public interface ICommandWithTwoParameters {
        IEnumerable<string> GetText(string parameter, int value);
    }
}