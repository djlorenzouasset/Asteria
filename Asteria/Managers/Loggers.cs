using System.Drawing;
using Pastel;

namespace Asteria.Managers;

public class Loggers
{
    # region colors 
    private Color paths = Color.FromArgb(16, 176, 125);
    private Color types = Color.FromArgb(168, 16, 176);
    private Color errors = Color.FromArgb(224, 16, 16);
    private Color warnings = Color.FromArgb(235, 211, 0);

    #endregion

    public string pathDefinition(string path) => path.Pastel(paths);
    public string typeDefinition(string type) => type.Pastel(types);
    public string errorDefinition(string error) => error.Pastel(errors);
    public string warnDefinition(string text) => text.Pastel(warnings);


}
