using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sauvgardes
{
    public static class Profil
    {
        static string nomutilisateur = null;

        public static string getNomUtilisateur()
        {
            return nomutilisateur;                
        }
        public static void setNomUtilisateur(string nom)
        {
            nomutilisateur = nom;
        }        
    }
}
