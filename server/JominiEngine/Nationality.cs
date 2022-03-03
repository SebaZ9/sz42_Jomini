﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
namespace JominiEngine
{
    /// <summary>
    /// Class storing data on nationality
    /// </summary>
    public class Nationality
    {
        /// <summary>
        /// Holds nationality ID
        /// </summary>
        public string natID { get; set; }
        /// <summary>
        /// Holds nationality name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Constructor for Nationality
        /// </summary>
        /// <param name="id">String holding nationality ID</param>
        /// <param name="nam">String holding nationality name</param>
        public Nationality(string id, string nam)
        {

            // TODO: validate id = string B,C,D,E,F,G,H,I,L/1-3

            // validate nam length = 1-20
            if ((nam.Length < 1) || (nam.Length > 20))
            {
                throw new InvalidDataException("Nationality name must be between 1 and 20 characters in length");
            }

            natID = id;
            name = nam;

        }

        /// <summary>
        /// Constructor for Nationality taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public Nationality()
        {
        }
			
    }
}
