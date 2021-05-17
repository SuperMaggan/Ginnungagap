using System;
using System.Collections.Generic;
using System.Linq;

namespace Bifrost.Common.Core.Domain
{
    public class AddDocument : IDocument
    {
        public string Id { get; set; }
        public string Domain { get; set; }

        /// <summary>
        /// This document's content in the form of key-value-pairs
        /// </summary>
        public List<Field> Fields { get; set; }


        public AddDocument()
        {
            Fields = new List<Field>();
        }

        

        public AddDocument(string id, string domain)
            : this()
        {
            Id = id;
            Domain = domain;
        }


        /// <summary>
        /// Get the field value of the field with the given name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="defaultIfNotFound"></param>
        /// <returns></returns>
        public string GetFieldValue(string fieldName, string defaultIfNotFound = "")
        {
            var field = Fields.FirstOrDefault(x => x.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
            if (field == null)
                return defaultIfNotFound;
            return field.Value;
        }

        /// <summary>
        /// Get the field values of the fields with the given name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public IEnumerable<string> GetFieldValues(string fieldName)
        {
            var fields = Fields.Where(x => x.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase)).Select(x => x.Value);
            return fields;
        }
        /// <summary>
        /// Apennds the value to the field with the given name
        /// If not existing, a new field will be created
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public void AppendToField(string fieldName, string value)
        {
            var field = Fields.FirstOrDefault(x => x.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
            if (field == null)
                Fields.Add(new Field(fieldName, value));
            else
                field.Value += value;
        }

        /// <summary>
        /// Sets the given value to an existing field with the given name
        /// if the field doesen't exist, it will be created
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public void SetField(string fieldName, string value)
        {
            var field = Fields.FirstOrDefault(x => x.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
            if (field == null)
                Fields.Add(new Field(fieldName, value));
            else
                field.Value = value;
        }

        /// <summary>
        /// Adds the given values to seperate fields with the same name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="values"></param>
        public void AddMultiFields(string fieldName, params string[] values)
        {
            foreach (var value in values)
            {
                Fields.Add(new Field(fieldName, value));
            }
        }

        /// <summary>
        /// Removes the fields matching the given fieldname
        /// </summary>
        /// <param name="fieldName"></param>
        public void RemoveFields(string fieldName)
        {
            Fields = Fields.Where(x => !x.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase)).ToList();
        }


        public IDocument Clone()
        {
            var doc = new AddDocument { Id = this.Id, Domain = this.Domain, Fields = new List<Field>() };
            if (Fields != null)
            {
                foreach (var field in Fields)
                {
                    doc.Fields.Add((Field)field.Clone());
                }
            }

            return doc;
        }
    }
}