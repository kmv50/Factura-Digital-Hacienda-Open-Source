using DataModel.EF;
using FacturaElectronica_V_4_3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Hacienda_Comunication
{
    public class FacturaDB_ToFacturaElectronica : HaciendaComunication
    {

        public FacturaDB_ToFacturaElectronica(Contribuyente contribuyente): base(contribuyente)
        {
        }

        public FacturaElectronica FacturaElectronica { get {
                return DocumentoElectronico as FacturaElectronica;
            }
        }
   
        public FacturaDB_ToFacturaElectronica Convertir(Factura facturaDB)
        {
            FacturaElectronica FacturaElectronica = new FacturaElectronica()
            {
                Clave = facturaDB.Clave,
                CondicionVenta = EnumUtils.SetTypeString<FacturaElectronicaCondicionVenta>(facturaDB.CondicionVenta),
                MedioPago = new FacturaElectronicaMedioPago[] {
                    EnumUtils.SetTypeString<FacturaElectronicaMedioPago>(facturaDB.MedioPago)
                },
                FechaEmision= facturaDB.Fecha_Emision_Documento,
                NumeroConsecutivo = new ConsecutivoHacienda() {
                    CasaMatriz = facturaDB.CasaMatriz,
                    PuntoVenta = facturaDB.PuntoVenta,
                    NumeracionConsecutiva = facturaDB.NumeroConsecutivo,
                    TipoDocumento = (Tipo_documento)facturaDB.Id_TipoDocumento 
                }.ToString(),    
                Emisor = new EmisorType() {
                    CorreoElectronico = facturaDB.Emisor_CorreoElectronico,
                    Identificacion = new IdentificacionType() {
                        Tipo = EnumUtils.SetTypeString<IdentificacionTypeTipo>(facturaDB.Emisor_Identificacion_Tipo),
                        Numero = facturaDB.Emisor_Identificacion_Numero
                    },
                    Nombre = facturaDB.Emisor_Nombre,
                    NombreComercial = facturaDB.Emisor_NombreComercial,
                    Telefono = new TelefonoType() {
                        CodigoPais = facturaDB.Emisor_Telefono_Codigo.Value.ToString(),
                        NumTelefono = facturaDB.Emisor_Telefono_Numero.Value.ToString()
                    },
                    Ubicacion = new UbicacionType() {
                        Barrio = facturaDB.Emisor_Ubicacion_Barrio.Value.ToString("00"),
                        Provincia = facturaDB.Emisor_Ubicacion_Provincia.Value.ToString(),
                        Canton = facturaDB.Emisor_Ubicacion_Canton.Value.ToString("00"),
                        Distrito = facturaDB.Emisor_Ubicacion_Distrito.Value.ToString("00"),
                        OtrasSenas = facturaDB.Emisor_Ubicacion_OtrasSenas ?? "No indicado"
                    }                  
                },     
                DetalleServicio = GetDetalleFromFacturaDB(facturaDB.Factura_Detalle).ToArray(),
                Receptor = GetReceptorFromFacturaDB(facturaDB),
                ResumenFactura = GetResumenFactura(facturaDB)
            };
            DocumentoElectronico = FacturaElectronica;



            requestData = new FacturaRequest()
            {
                clave = FacturaElectronica.Clave,
                emisor = new FacturaClient()
                {
                    numeroIdentificacion = FacturaElectronica.Emisor.Identificacion.Numero,
                    tipoIdentificacion = FacturaElectronica.Emisor.Identificacion.Tipo.GetXmlValue()
                },
                fecha = FacturaElectronica.FechaEmision.ToString("yyyy-MM-ddTHH:mm:ss"),
            };

            if(FacturaElectronica.Receptor != null && FacturaElectronica.Receptor.Identificacion != null)
            {
                requestData.receptor = new FacturaClient()
                {
                    tipoIdentificacion = FacturaElectronica.Receptor.Identificacion.Tipo.GetXmlValue(),
                    numeroIdentificacion = FacturaElectronica.Receptor.Identificacion.Numero,
                };
            }

            return this;
        }

        private FacturaElectronicaResumenFactura GetResumenFactura(Factura fac)
        {
            FacturaElectronicaResumenFactura resumen = new FacturaElectronicaResumenFactura();
            //if (fac.Codigo_Moneda == "CRC")
            //{
            resumen.CodigoTipoMoneda = new CodigoMonedaType()
            {
                CodigoMoneda = CodigoMonedaTypeCodigoMoneda.CRC,
                TipoCambio = 0
            };
            //}
            //else
            //{
            //    resumen.CodigoMoneda = FacturaElectronicaResumenFacturaCodigoMoneda.USD;
            //    resumen.CodigoMonedaSpecified = true;
            //    resumen.TipoCambio = fac.Tipo_Cambio;
            //    resumen.TipoCambioSpecified = true;
            //}


            resumen.TotalComprobante = fac.TotalComprobante;


            if (fac.TotalDescuentos != null && fac.TotalDescuentos > 0)
            {
                resumen.TotalDescuentos = fac.TotalDescuentos.Value;
                resumen.TotalDescuentosSpecified = true;
            }

            if (fac.TotalExento != null && fac.TotalExento > 0)
            {
                resumen.TotalExento = fac.TotalExento.Value;
                resumen.TotalExentoSpecified = true;
            }

            if (fac.TotalGravado != null && fac.TotalGravado > 0)
            {
                resumen.TotalGravado = fac.TotalGravado.Value;
                resumen.TotalGravadoSpecified = true;
            }

            if (fac.TotalImpuesto != null && fac.TotalImpuesto > 0)
            {
                resumen.TotalImpuesto = fac.TotalImpuesto.Value;
                resumen.TotalImpuestoSpecified = true;
            }
            resumen.TotalVentaNeta = fac.TotalVentaNeta;
            resumen.TotalVenta = fac.TotalExento.Value + fac.TotalGravado.Value;

            if (fac.TotalMercanciasExentas != null && fac.TotalMercanciasExentas > 0)
            {
                resumen.TotalMercanciasExentas = fac.TotalMercanciasExentas.Value;
                resumen.TotalMercanciasExentasSpecified = true;
            }

            if (fac.TotalMercanciasGravadas != null && fac.TotalMercanciasGravadas > 0)
            {
                resumen.TotalMercanciasGravadas = fac.TotalMercanciasGravadas.Value;
                resumen.TotalMercanciasGravadasSpecified = true;
            }

            //////////////////////////////////////

            if (fac.TotalServExentos != null && fac.TotalServExentos > 0)
            {
                resumen.TotalServExentos = fac.TotalServExentos.Value;
                resumen.TotalServExentosSpecified = true;
            }

            if (fac.TotalServGravados != null && fac.TotalServGravados > 0)
            {
                resumen.TotalServGravados = fac.TotalServGravados.Value;
                resumen.TotalServGravadosSpecified = true;
            }

            return resumen;
        }


        private ReceptorType GetReceptorFromFacturaDB(Factura facturaDB)
        {
            try
            {
                ReceptorType e = new ReceptorType();

                e.CorreoElectronico = facturaDB.Receptor_CorreoElectronico;

                if (facturaDB.Receptor_Telefono_Codigo != null && facturaDB.Receptor_Telefono_Numero != null)
                    e.Telefono = new TelefonoType()
                    {
                        CodigoPais = facturaDB.Receptor_Telefono_Codigo.Value.ToString(),
                        NumTelefono = facturaDB.Receptor_Telefono_Numero.Value.ToString()
                    };


                e.Nombre = facturaDB.Receptor_Nombre;
                e.NombreComercial = facturaDB.Receptor_NombreComercial;
                if (facturaDB.Receptor_Ubicacion_Barrio != null && facturaDB.Receptor_Ubicacion_Canton != null && facturaDB.Receptor_Ubicacion_Provincia != null && facturaDB.Receptor_Ubicacion_Distrito != null)
                    e.Ubicacion = new UbicacionType()
                    {
                        Barrio = facturaDB.Receptor_Ubicacion_Barrio.Value.ToString("00"),
                        Canton = facturaDB.Receptor_Ubicacion_Canton.Value.ToString("00"),
                        Distrito = facturaDB.Receptor_Ubicacion_Distrito.Value.ToString("00"),
                        OtrasSenas = facturaDB.Receptor_Ubicacion_OtrasSenas == null ? "No indicado" : facturaDB.Receptor_Ubicacion_OtrasSenas,
                        Provincia = facturaDB.Receptor_Ubicacion_Provincia.Value.ToString()
                    };

                if (!String.IsNullOrEmpty(facturaDB.Receptor_Identificacion_Numero) && facturaDB.Receptor_Identificacion_Numero.Length > 0 && facturaDB.Receptor_Identificacion_Tipo != null && facturaDB.Receptor_Identificacion_Tipo != "")
                {
                    if (facturaDB.Receptor_Identificacion_Tipo != "EX")
                    {
                        e.Identificacion = new IdentificacionType()
                        {
                            Numero = facturaDB.Receptor_Identificacion_Numero,
                            Tipo = EnumUtils.SetTypeString<IdentificacionTypeTipo>(facturaDB.Receptor_Identificacion_Tipo)
                        };
                    }
                    else
                    {
                        e.IdentificacionExtranjero = facturaDB.Receptor_Identificacion_Numero;
                    }

                }

                return e;
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                return new ReceptorType()
                {
                    Nombre = facturaDB.Receptor_Nombre,
                    CorreoElectronico = facturaDB.Receptor_CorreoElectronico
                };
            }
        }

        private List<FacturaElectronicaLineaDetalle> GetDetalleFromFacturaDB(ICollection<Factura_Detalle> items)
        {
            List<FacturaElectronicaLineaDetalle> lst = new List<FacturaElectronicaLineaDetalle>();
            int NumeroLinea = 1;
            foreach (Factura_Detalle q in items)
            {


                FacturaElectronicaLineaDetalle fd = new FacturaElectronicaLineaDetalle()
                {
                    Cantidad = q.Cantidad,
                    CodigoComercial = new CodigoType[]{
                        new CodigoType(){
                            Codigo = q.Codigo,
                            Tipo = CodigoTypeTipo.Item01
                        }
                    },
                    Detalle = q.ProductoServicio,                    
                    NumeroLinea = NumeroLinea.ToString(),
                    PrecioUnitario = q.PrecioUnitario,
                    SubTotal = q.SubTotal,
                    UnidadMedida = EnumUtils.SetTypeString<UnidadMedidaType>(q.Unidad_Medida),
                    MontoTotal = q.Monto_Total,
                    MontoTotalLinea = q.Monto_Total_Linea,


                };

                if (q.Monto_Descuento.HasValue && q.Monto_Descuento.Value != 0)
                {
                    fd.Descuento = new DescuentoType[] {
                        new DescuentoType(){
                            MontoDescuento = q.Monto_Descuento.Value,
                            NaturalezaDescuento = string.IsNullOrEmpty(q.Naturaleza_Descuento) ? "No se indica" : q.Naturaleza_Descuento
                        }
                    };                   
                }

            


                if (q.Factura_Detalle_Impuesto != null && q.Factura_Detalle_Impuesto.Count > 0)
                {
                    List<ImpuestoType> impuestoD = new List<ImpuestoType>();
                    foreach (Factura_Detalle_Impuesto impuesto in q.Factura_Detalle_Impuesto)
                    {
                        if (impuesto.Exento)
                        {
                            //exento parcial
                            if (!string.IsNullOrWhiteSpace(impuesto.Exoneracion_PorcentajeCompra) && impuesto.Exoneracion_MontoImpuesto.HasValue)//exento parcial
                            {

                                ImpuestoType impuestoHacienda = new ImpuestoType()
                                {
                                    Codigo = EnumUtils.SetTypeString<ImpuestoTypeCodigo>(impuesto.Impuesto_Codigo),
                                    Monto = impuesto.Impuesto_Monto,
                                    TarifaSpecified = true,
                                    Tarifa = impuesto.Impuesto_Tarifa,                                    
                                    Exoneracion = new ExoneracionType()
                                    {
                                        FechaEmision = impuesto.Exoneracion_FechaEmision.Value,
                                        MontoExoneracion = impuesto.Exoneracion_MontoImpuesto.Value,
                                        NombreInstitucion = impuesto.Exoneracion_NombreInstitucion,
                                        NumeroDocumento = impuesto.Exoneracion_NumeroDocumento,
                                        PorcentajeExoneracion = impuesto.Exoneracion_PorcentajeCompra,
                                        TipoDocumento = EnumUtils.SetTypeString<ExoneracionTypeTipoDocumento>(impuesto.Exoneracion_TipoDocumento)
                                    },
                                    
                                };

                                if (!string.IsNullOrEmpty(impuesto.CodigoTarifa)) {
                                    impuestoHacienda.CodigoTarifa = EnumUtils.SetTypeString<ImpuestoTypeCodigoTarifa>(impuesto.CodigoTarifa);
                                    impuestoHacienda.CodigoTarifaSpecified = true;
                                }
                                else {
                                    impuestoHacienda.CodigoTarifaSpecified = false;
                                }

                                if (impuesto.FactorIVA.HasValue && impuesto.FactorIVA > 0) {
                                    impuestoHacienda.FactorIVA = impuesto.FactorIVA.Value;
                                }


                                impuestoD.Add(impuestoHacienda);

                            }
                            else if (impuesto.Impuesto_Tarifa != 0) //exento linea total todo a 0 
                            {
                                impuestoD.Add(new ImpuestoType()
                                {
                                    Codigo = EnumUtils.SetTypeString<ImpuestoTypeCodigo>(impuesto.Impuesto_Codigo),
                                    Monto = 0,
                                    Tarifa = 0,
                                });
                            }
                        }
                        else//gravado
                        {
                            impuestoD.Add(new ImpuestoType()
                            {
                                Codigo = EnumUtils.SetTypeString<ImpuestoTypeCodigo>(impuesto.Impuesto_Codigo),
                                Monto = impuesto.Impuesto_Monto,
                                Tarifa = impuesto.Impuesto_Tarifa,
                            });
                        }
                    }

                    if (impuestoD.Count != 0)
                        fd.Impuesto = impuestoD.ToArray();
                }


                lst.Add(fd);
                NumeroLinea++;
            }

            return lst;
        }
       
    }
}
