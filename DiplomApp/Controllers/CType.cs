using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomApp
{
    /// <summary>
    /// Представляет перечисление всех поддерживаемых типов микроконтроллеров
    /// </summary>
    [Flags]
    public enum CType
    {
        Switch,
        Termometer
    }
}
