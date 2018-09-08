using DataModel.EF;
using NotaCreditoElectronica_V4_2;
using System;
using System.Collections.Generic;

namespace DataModel.Hacienda_Comunication
{
    public class FacturaDB_ToNotaCredito : HaciendaComunication
    {

        public FacturaDB_ToNotaCredito(Contribuyente contribuyente) : base(contribuyente)
        {
        }

        public NotaCreditoElectronica FacturaElectronica => DocumentoElectronico as NotaCreditoElectronica;

        public FacturaDB_ToNotaCredito Convertir(Factura facturaDB , DateTime FechaEmisionOriginal)
        {
            if (!facturaDB.InformacionReferencia_Codigo.HasValue || !facturaDB.InformacionReferencia_FechaEmision.HasValue)
                throw new Exception("Informacion de referencia incompleta");

            NotaCreditoElectronica NotaCreditoElectronica = new NotaCreditoElectronica()
            {
                Clave = facturaDB.Clave,
                CondicionVenta = EnumUtils.SetTypeString<NotaCreditoElectronicaCondicionVenta>(facturaDB.CondicionVenta),
                MedioPago = new NotaCreditoElectronicaMedioPago[] {
                    EnumUtils.SetTypeString<NotaCreditoElectronicaMedioPago>(facturaDB.MedioPago)
                },
                FechaEmision = FechaEmisionOriginal,
                NumeroConsecutivo = new ConsecutivoHacienda()
                {
                    CasaMatriz = facturaDB.CasaMatriz,
                    PuntoVenta = facturaDB.PuntoVenta,
                    NumeracionConsecutiva = facturaDB.NumeroConsecutivo,
                    TipoDocumento = (Tipo_documento)facturaDB.Id_TipoDocumento
                }.ToString(),
                Emisor = new NotaCreditoElectronica_V4_2.EmisorType()
                {
                    CorreoElectronico = facturaDB.Emisor_CorreoElectronico,
                    Identificacion = new NotaCreditoElectronica_V4_2.IdentificacionType()
                    {
                        Tipo = EnumUtils.SetTypeString<NotaCreditoElectronica_V4_2.IdentificacionTypeTipo>(facturaDB.Emisor_Identificacion_Tipo),
                        Numero = facturaDB.Emisor_Identificacion_Numero
                    },
                    Nombre = facturaDB.Emisor_Nombre,
                    NombreComercial = facturaDB.Emisor_NombreComercial,
                    Telefono = new NotaCreditoElectronica_V4_2.TelefonoType()
                    {
                        CodigoPais = facturaDB.Emisor_Telefono_Codigo.Value.ToString(),
                        NumTelefono = facturaDB.Emisor_Telefono_Numero.Value.ToString()
                    },
                    Ubicacion = new NotaCreditoElectronica_V4_2.UbicacionType()
                    {
                        Barrio = facturaDB.Emisor_Ubicacion_Barrio.Value.ToString("00"),
                        Provincia = facturaDB.Emisor_Ubicacion_Provincia.Value.ToString(),
                        Canton = facturaDB.Emisor_Ubicacion_Canton.Value.ToString("00"),
                        Distrito = facturaDB.Emisor_Ubicacion_Distrito.Value.ToString("00"),
                        OtrasSenas = facturaDB.Emisor_Ubicacion_OtrasSenas ?? "No indicado"
                    }
                },
                Normativa = new NotaCreditoElectronicaNormativa()
                {
                    NumeroResolucion = "DGT-R-48-2016",
                    FechaResolucion = facturaDB.Fecha_Emision_Documento.ToString("dd-MM-yyyy HH:mm:ss")
                },
                DetalleServicio = GetDetalleFromFacturaDB(facturaDB.Factura_Detalle).ToArray(),
                Receptor = GetReceptorFromFacturaDB(facturaDB),
                ResumenFactura = GetResumenFactura(facturaDB)
            };

           
            NotaCreditoElectronica.InformacionReferencia = new NotaCreditoElectronicaInformacionReferencia[]
            {
                new NotaCreditoElectronicaInformacionReferencia(){
                    TipoDoc = NotaCreditoElectronicaInformacionReferenciaTipoDoc.Item03,
                    Numero = facturaDB.InformacionReferencia_Numero,
                    Razon = facturaDB.InformacionReferencia_Razon,
                    Codigo =  EnumUtils.SetTypeString<NotaCreditoElectronicaInformacionReferenciaCodigo>(facturaDB.InformacionReferencia_Codigo.Value.ToString("00")),
                    FechaEmision = facturaDB.InformacionReferencia_FechaEmision.Value
                }
            };

            DocumentoElectronico = NotaCreditoElectronica;



            requestData = new FacturaRequest()
            {
                clave = NotaCreditoElectronica.Clave,
                emisor = new FacturaClient()
                {
                    numeroIdentificacion = NotaCreditoElectronica.Emisor.Identificacion.Numero,
                    tipoIdentificacion = NotaCreditoElectronica.Emisor.Identificacion.Tipo.GetXmlValue()
                },
                fecha = NotaCreditoElectronica.FechaEmision.ToString("yyyy-MM-ddTHH:mm:ss"),
            };

            if (NotaCreditoElectronica.Receptor != null && NotaCreditoElectronica.Receptor.Identificacion != null)
            {
                requestData.receptor = new FacturaClient()
                {
                    tipoIdentificacion = NotaCreditoElectronica.Receptor.Identificacion.Tipo.GetXmlValue(),
                    numeroIdentificacion = NotaCreditoElectronica.Receptor.Identificacion.Numero,
                };
            }

            return this;
        }

        private NotaCreditoElectronicaResumenFactura GetResumenFactura(Factura fac)
        {
            NotaCreditoElectronicaResumenFactura resumen = new NotaCreditoElectronicaResumenFactura
            {
                //if (fac.Codigo_Moneda == "CRC")
                //{
                CodigoMoneda = NotaCreditoElectronicaResumenFacturaCodigoMoneda.CRC,
                CodigoMonedaSpecified = true,
                //}
                //else
                //{
                //    resumen.CodigoMoneda = FacturaElectronicaResumenFacturaCodigoMoneda.USD;
                //    resumen.CodigoMonedaSpecified = true;
                //    resumen.TipoCambio = fac.Tipo_Cambio;
                //    resumen.TipoCambioSpecified = true;
                //}


                TotalComprobante = fac.TotalComprobante
            };


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


        private NotaCreditoElectronica_V4_2.ReceptorType GetReceptorFromFacturaDB(Factura facturaDB)
        {
            try
            {
                NotaCreditoElectronica_V4_2.ReceptorType e = new NotaCreditoElectronica_V4_2.ReceptorType
                {
                    CorreoElectronico = facturaDB.Receptor_CorreoElectronico
                };

                if (facturaDB.Receptor_Telefono_Codigo != null && facturaDB.Receptor_Telefono_Numero != null)
                {
                    e.Telefono = new NotaCreditoElectronica_V4_2.TelefonoType()
                    {
                        CodigoPais = facturaDB.Receptor_Telefono_Codigo.Value.ToString(),
                        NumTelefono = facturaDB.Receptor_Telefono_Numero.Value.ToString()
                    };
                }

                e.Nombre = facturaDB.Receptor_Nombre;
                e.NombreComercial = facturaDB.Receptor_NombreComercial;
                if (facturaDB.Receptor_Ubicacion_Barrio != null && facturaDB.Receptor_Ubicacion_Canton != null && facturaDB.Receptor_Ubicacion_Provincia != null && facturaDB.Receptor_Ubicacion_Distrito != null)
                {
                    e.Ubicacion = new NotaCreditoElectronica_V4_2.UbicacionType()
                    {
                        Barrio = facturaDB.Receptor_Ubicacion_Barrio.Value.ToString("00"),
                        Canton = facturaDB.Receptor_Ubicacion_Canton.Value.ToString("00"),
                        Distrito = facturaDB.Receptor_Ubicacion_Distrito.Value.ToString("00"),
                        OtrasSenas = facturaDB.Receptor_Ubicacion_OtrasSenas == null ? "No indicado" : facturaDB.Receptor_Ubicacion_OtrasSenas,
                        Provincia = facturaDB.Receptor_Ubicacion_Provincia.Value.ToString()
                    };
                }

                if (!string.IsNullOrEmpty(facturaDB.Receptor_Identificacion_Numero) && facturaDB.Receptor_Identificacion_Numero.Length > 0 && facturaDB.Receptor_Identificacion_Tipo != null && facturaDB.Receptor_Identificacion_Tipo != "")
                {
                    if (facturaDB.Receptor_Identificacion_Tipo != "EX")
                    {
                        e.Identificacion = new NotaCreditoElectronica_V4_2.IdentificacionType()
                        {
                            Numero = facturaDB.Receptor_Identificacion_Numero,
                            Tipo = EnumUtils.SetTypeString<NotaCreditoElectronica_V4_2.IdentificacionTypeTipo>(facturaDB.Receptor_Identificacion_Tipo)
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
                return new NotaCreditoElectronica_V4_2.ReceptorType()
                {
                    Nombre = facturaDB.Receptor_Nombre,
                    CorreoElectronico = facturaDB.Receptor_CorreoElectronico
                };
            }
        }

        private List<NotaCreditoElectronicaLineaDetalle> GetDetalleFromFacturaDB(ICollection<Factura_Detalle> items)
        {
            List<NotaCreditoElectronicaLineaDetalle> lst = new List<NotaCreditoElectronicaLineaDetalle>();
            int NumeroLinea = 1;
            foreach (Factura_Detalle q in items)
            {


                NotaCreditoElectronicaLineaDetalle fd = new NotaCreditoElectronicaLineaDetalle()
                {
                    Cantidad = q.Cantidad,
                    Codigo = new NotaCreditoElectronica_V4_2.CodigoType[]{
                        new NotaCreditoElectronica_V4_2.CodigoType(){
                            Codigo = q.Codigo,
                            Tipo = NotaCreditoElectronica_V4_2.CodigoTypeTipo.Item01
                        }
                    },
                    Detalle = q.ProductoServicio,
                    MontoDescuentoSpecified = false,
                    NumeroLinea = NumeroLinea.ToString(),
                    PrecioUnitario = q.PrecioUnitario,
                    SubTotal = q.SubTotal,
                    UnidadMedida = EnumUtils.SetTypeString<NotaCreditoElectronica_V4_2.UnidadMedidaType>(q.Unidad_Medida),
                    MontoTotal = q.Monto_Total,
                    MontoTotalLinea = q.Monto_Total_Linea,


                };

                if (q.Monto_Descuento.HasValue && q.Monto_Descuento.Value != 0)
                {
                    fd.MontoDescuento = q.Monto_Descuento.Value;
                    fd.MontoDescuentoSpecified = true;
                    if (string.IsNullOrEmpty(q.Naturaleza_Descuento))
                    {
                        fd.NaturalezaDescuento = "No se indica";
                    }
                    else
                    {
                        fd.NaturalezaDescuento = q.Naturaleza_Descuento;
                    }
                }




                if (q.Factura_Detalle_Impuesto != null && q.Factura_Detalle_Impuesto.Count > 0)
                {
                    List<NotaCreditoElectronica_V4_2.ImpuestoType> impuestoD = new List<NotaCreditoElectronica_V4_2.ImpuestoType>();
                    foreach (Factura_Detalle_Impuesto impuesto in q.Factura_Detalle_Impuesto)
                    {
                        if (impuesto.Exento)
                        {
                            //exento parcial
                            if (!string.IsNullOrWhiteSpace(impuesto.Exoneracion_PorcentajeCompra) && impuesto.Exoneracion_MontoImpuesto.HasValue)//exento parcial
                            {

                                impuestoD.Add(new NotaCreditoElectronica_V4_2.ImpuestoType()
                                {
                                    Codigo = EnumUtils.SetTypeString<NotaCreditoElectronica_V4_2.ImpuestoTypeCodigo>(impuesto.Impuesto_Codigo),
                                    Monto = impuesto.Impuesto_Monto,
                                    Tarifa = impuesto.Impuesto_Tarifa,
                                    Exoneracion = new NotaCreditoElectronica_V4_2.ExoneracionType()
                                    {
                                        FechaEmision = impuesto.Exoneracion_FechaEmision.Value,
                                        MontoImpuesto = impuesto.Exoneracion_MontoImpuesto.Value,
                                        NombreInstitucion = impuesto.Exoneracion_NombreInstitucion,
                                        NumeroDocumento = impuesto.Exoneracion_NumeroDocumento,
                                        PorcentajeCompra = impuesto.Exoneracion_PorcentajeCompra,
                                        TipoDocumento = EnumUtils.SetTypeString<NotaCreditoElectronica_V4_2.ExoneracionTypeTipoDocumento>(impuesto.Exoneracion_TipoDocumento)
                                    }
                                });

                            }
                            else if (impuesto.Impuesto_Tarifa != 0) //exento linea total todo a 0 
                            {
                                impuestoD.Add(new NotaCreditoElectronica_V4_2.ImpuestoType()
                                {
                                    Codigo = EnumUtils.SetTypeString<NotaCreditoElectronica_V4_2.ImpuestoTypeCodigo>(impuesto.Impuesto_Codigo),
                                    Monto = 0,
                                    Tarifa = 0,
                                });
                            }
                        }
                        else//gravado
                        {
                            impuestoD.Add(new NotaCreditoElectronica_V4_2.ImpuestoType()
                            {
                                Codigo = EnumUtils.SetTypeString<NotaCreditoElectronica_V4_2.ImpuestoTypeCodigo>(impuesto.Impuesto_Codigo),
                                Monto = impuesto.Impuesto_Monto,
                                Tarifa = impuesto.Impuesto_Tarifa,
                            });
                        }
                    }

                    if (impuestoD.Count != 0)
                    {
                        fd.Impuesto = impuestoD.ToArray();
                    }
                }


                lst.Add(fd);
                NumeroLinea++;
            }

            return lst;
        }

    }
}

