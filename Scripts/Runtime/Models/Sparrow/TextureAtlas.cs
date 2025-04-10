using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Exprite
{
    [XmlRoot(ElementName="TextureAtlas")]
    public class TextureAtlas { 

        [XmlElement(ElementName="SubTexture")] 
        public List<SubTexture> SubTexture { get; set; } 

        [XmlAttribute(AttributeName="imagePath")] 
        public string imagePath { get; set; } 
    }
}

