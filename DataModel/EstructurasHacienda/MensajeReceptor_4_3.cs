﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// Este código fuente fue generado automáticamente por xsd, Versión=4.6.1055.0.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://cdn.comprobanteselectronicos.go.cr/xml-schemas/v4.3/mensajeReceptor")]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "https://cdn.comprobanteselectronicos.go.cr/xml-schemas/v4.3/mensajeReceptor", IsNullable = false)]
public partial class MensajeReceptor
{

    private string claveField;

    private string numeroCedulaEmisorField;

    private System.DateTime fechaEmisionDocField;

    private MensajeReceptorMensaje mensajeField;

    private string detalleMensajeField;

    private decimal montoTotalImpuestoField;

    private bool montoTotalImpuestoFieldSpecified;

    private string codigoActividadField;

    private MensajeReceptorCondicionImpuesto condicionImpuestoField;

    private bool condicionImpuestoFieldSpecified;

    private decimal montoTotalImpuestoAcreditarField;

    private bool montoTotalImpuestoAcreditarFieldSpecified;

    private decimal montoTotalDeGastoAplicableField;

    private bool montoTotalDeGastoAplicableFieldSpecified;

    private decimal totalFacturaField;

    private string numeroCedulaReceptorField;

    private string numeroConsecutivoReceptorField;

    /// <remarks/>
    public string Clave
    {
        get
        {
            return this.claveField;
        }
        set
        {
            this.claveField = value;
        }
    }

    /// <remarks/>
    public string NumeroCedulaEmisor
    {
        get
        {
            return this.numeroCedulaEmisorField;
        }
        set
        {
            this.numeroCedulaEmisorField = value;
        }
    }

    /// <remarks/>
    public System.DateTime FechaEmisionDoc
    {
        get
        {
            return this.fechaEmisionDocField;
        }
        set
        {
            this.fechaEmisionDocField = value;
        }
    }

    /// <remarks/>
    public MensajeReceptorMensaje Mensaje
    {
        get
        {
            return this.mensajeField;
        }
        set
        {
            this.mensajeField = value;
        }
    }

    /// <remarks/>
    public string DetalleMensaje
    {
        get
        {
            return this.detalleMensajeField;
        }
        set
        {
            this.detalleMensajeField = value;
        }
    }

    /// <remarks/>
    public decimal MontoTotalImpuesto
    {
        get
        {
            return this.montoTotalImpuestoField;
        }
        set
        {
            this.montoTotalImpuestoField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool MontoTotalImpuestoSpecified
    {
        get
        {
            return this.montoTotalImpuestoFieldSpecified;
        }
        set
        {
            this.montoTotalImpuestoFieldSpecified = value;
        }
    }

    /// <remarks/>
    public string CodigoActividad
    {
        get
        {
            return this.codigoActividadField;
        }
        set
        {
            this.codigoActividadField = value;
        }
    }

    /// <remarks/>
    public MensajeReceptorCondicionImpuesto CondicionImpuesto
    {
        get
        {
            return this.condicionImpuestoField;
        }
        set
        {
            this.condicionImpuestoField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool CondicionImpuestoSpecified
    {
        get
        {
            return this.condicionImpuestoFieldSpecified;
        }
        set
        {
            this.condicionImpuestoFieldSpecified = value;
        }
    }

    /// <remarks/>
    public decimal MontoTotalImpuestoAcreditar
    {
        get
        {
            return this.montoTotalImpuestoAcreditarField;
        }
        set
        {
            this.montoTotalImpuestoAcreditarField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool MontoTotalImpuestoAcreditarSpecified
    {
        get
        {
            return this.montoTotalImpuestoAcreditarFieldSpecified;
        }
        set
        {
            this.montoTotalImpuestoAcreditarFieldSpecified = value;
        }
    }

    /// <remarks/>
    public decimal MontoTotalDeGastoAplicable
    {
        get
        {
            return this.montoTotalDeGastoAplicableField;
        }
        set
        {
            this.montoTotalDeGastoAplicableField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool MontoTotalDeGastoAplicableSpecified
    {
        get
        {
            return this.montoTotalDeGastoAplicableFieldSpecified;
        }
        set
        {
            this.montoTotalDeGastoAplicableFieldSpecified = value;
        }
    }

    /// <remarks/>
    public decimal TotalFactura
    {
        get
        {
            return this.totalFacturaField;
        }
        set
        {
            this.totalFacturaField = value;
        }
    }

    /// <remarks/>
    public string NumeroCedulaReceptor
    {
        get
        {
            return this.numeroCedulaReceptorField;
        }
        set
        {
            this.numeroCedulaReceptorField = value;
        }
    }

    /// <remarks/>
    public string NumeroConsecutivoReceptor
    {
        get
        {
            return this.numeroConsecutivoReceptorField;
        }
        set
        {
            this.numeroConsecutivoReceptorField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://cdn.comprobanteselectronicos.go.cr/xml-schemas/v4.3/mensajeReceptor")]
public enum MensajeReceptorMensaje
{

    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("1")]
    Item1,

    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("2")]
    Item2,

    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("3")]
    Item3,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://cdn.comprobanteselectronicos.go.cr/xml-schemas/v4.3/mensajeReceptor")]
public enum MensajeReceptorCondicionImpuesto
{

    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("01")]
    Item01,

    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("02")]
    Item02,

    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("03")]
    Item03,

    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("04")]
    Item04,

    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("05")]
    Item05,
}