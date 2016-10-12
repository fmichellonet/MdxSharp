using Microsoft.AnalysisServices.AdomdClient;

namespace MdxSharp
{

    public static class AdomdConnectionExtensions
    {
        public static Query<TDef> Query<TDef>(this AdomdConnection cnx) 
            where TDef : class
        {
            return new Query<TDef>(cnx, null, null);
        }
    }
}
