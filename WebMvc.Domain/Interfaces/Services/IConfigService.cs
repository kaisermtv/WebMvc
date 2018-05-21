using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMvc.Domain.Interfaces.Services
{
    public partial interface IConfigService
    {
        #region Emojies

        string Emotify(string inputText);
        OrderedDictionary GetEmoticonHashTable();
        Dictionary<string, string> GetForumConfig();
        Dictionary<string, string> GetTypes();

        #endregion
    }
}
