﻿using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Xml.Serialization;
using KB.SharpCore.Utils;

namespace KB.SharpCore.Serialization;

public static class XmlSerializableHelper
{
    public static Result Save<T>(T obj, string path)
    {
        try
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(fileStream, obj);
                return Result.CreateSuccess();
            }
        }
        catch (Exception e)
        {
            return Result.CreateFailure(e);
        }
    }

    public static Result<T> Load<T>(string path)
    {
        using (FileStream fileStream = new FileStream(path, FileMode.Open))
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                return Result<T>.CreateSuccess((T)xmlSerializer.Deserialize(fileStream)!);
            }
            catch (Exception e)
            {
                return Result<T>.CreateFailure(e);
            }
        }
    }

    public static Result Load<T>(string path, [NotNull] T toFillObject)
        where T : class
    {
        if (toFillObject == null)
        {
            throw new ArgumentNullException(nameof(toFillObject));
        }

        Result<T> deserializedResult = XmlSerializableHelper.Load<T>(path);
        if (deserializedResult.IsSuccess)
        {
            XmlSerializableHelper._Load(toFillObject, deserializedResult.Value);
        }

        return deserializedResult.ToResult();
    }

    public static Result<string> SaveToXMLString<T>(T dataObject)
    {
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            // Create a StringWriter to hold the XML data
            StringWriter writer = new StringWriter();
            // Serialize the object to XML
            serializer.Serialize(writer, dataObject);

            return Result<string>.CreateSuccess(writer.ToString());
        }
        catch(Exception e)
        {
            return Result<string>.CreateFailure(e);
        }
    }

    public static Result<T> LoadFromXMLString<T>(string xml)
    {
        try
        {
            // Create an XmlSerializer for your object type
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            // Create a StringReader to read the XML data
            using (StringReader reader = new StringReader(xml))
            {
                // Deserialize the XML to an object
                return Result<T>.CreateSuccess((T)serializer.Deserialize(reader)!);
            }
        }
        catch(Exception e)
        {
            return Result<T>.CreateFailure(e);
        }
    }

    private static void _Load<T>(T toFillObject, T dataObject)
    {
        IList<PropertyInfo> props = new List<PropertyInfo>(typeof(T).GetProperties());
        foreach (PropertyInfo prop in props)
        {
            // Check if it is a collection
            if (prop.PropertyType.IsGenericType)
            {
                // Get the type of the list
                Type itemType = prop.PropertyType.GetGenericArguments()[0];
                // Create an instance of that type
                object? item = Activator.CreateInstance(itemType);
                // Add it to the list
                prop.SetValue(toFillObject, item, null);
            }
            else
            {
                // Get the value of the property from the file
                object? value = prop.GetValue(dataObject, null);
                // Set the value of the property to this instance
                prop.SetValue(toFillObject, value, null);
            }
        }
    }
}