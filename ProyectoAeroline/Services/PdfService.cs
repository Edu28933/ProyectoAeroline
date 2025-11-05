using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ProyectoAeroline.Data;

namespace ProyectoAeroline.Services
{
    public class PdfService : IPdfService
    {
        public PdfService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerarPdfBoleto(BoletosData.BoletoCompletoInfo boletoInfo)
        {
            if (boletoInfo == null || boletoInfo.Boleto == null)
                throw new ArgumentNullException(nameof(boletoInfo));

            var boleto = boletoInfo.Boleto;
            var vuelo = boletoInfo.Vuelo ?? new Models.VuelosModel();
            var pasajero = boletoInfo.Pasajero ?? new Models.PasajerosModel();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .AlignCenter()
                        .Text("🎫 BOLETO DE VUELO")
                        .SemiBold()
                        .FontSize(20)
                        .FontColor(Colors.Blue.Darken3);

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(15);

                            // Información del Boleto
                            column.Item().Element(container =>
                            {
                                container
                                    .Background(Colors.Blue.Lighten5)
                                    .Padding(15)
                                    .Border(1)
                                    .BorderColor(Colors.Blue.Darken1)
                                    .Column(col =>
                                    {
                                        col.Item().Text("INFORMACIÓN DEL BOLETO").Bold().FontSize(14);
                                        col.Item().PaddingTop(5).Row(row =>
                                        {
                                            row.ConstantItem(120).Text("ID Boleto:").SemiBold();
                                            row.RelativeItem().Text($"#{boleto.IdBoleto}");
                                        });
                                        col.Item().Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Fecha de Compra:").SemiBold();
                                            row.RelativeItem().Text(boleto.FechaCompra.ToString("dd/MM/yyyy HH:mm"));
                                        });
                                        col.Item().Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Estado:").SemiBold();
                                            row.RelativeItem().Text(boleto.Estado ?? "N/A");
                                        });
                                    });
                            });

                            // Información del Pasajero
                            column.Item().Element(container =>
                            {
                                container
                                    .Background(Colors.Green.Lighten5)
                                    .Padding(15)
                                    .Border(1)
                                    .BorderColor(Colors.Green.Darken1)
                                    .Column(col =>
                                    {
                                        col.Item().Text("INFORMACIÓN DEL PASAJERO").Bold().FontSize(14);
                                        col.Item().PaddingTop(5).Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Nombre:").SemiBold();
                                            row.RelativeItem().Text($"{pasajero.Nombres} {pasajero.Apellidos}".Trim());
                                        });
                                        col.Item().Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Pasaporte:").SemiBold();
                                            row.RelativeItem().Text(pasajero.Pasaporte ?? "N/A");
                                        });
                                        col.Item().Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Correo:").SemiBold();
                                            row.RelativeItem().Text(pasajero.Correo ?? "N/A");
                                        });
                                        col.Item().Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Teléfono:").SemiBold();
                                            row.RelativeItem().Text(pasajero.Telefono?.ToString() ?? "N/A");
                                        });
                                    });
                            });

                            // Información del Vuelo
                            column.Item().Element(container =>
                            {
                                container
                                    .Background(Colors.Orange.Lighten5)
                                    .Padding(15)
                                    .Border(1)
                                    .BorderColor(Colors.Orange.Darken1)
                                    .Column(col =>
                                    {
                                        col.Item().Text("INFORMACIÓN DEL VUELO").Bold().FontSize(14);
                                        col.Item().PaddingTop(5).Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Aerolínea:").SemiBold();
                                            row.RelativeItem().Text(vuelo.Aerolinea ?? "N/A");
                                        });
                                        col.Item().Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Número de Vuelo:").SemiBold();
                                            row.RelativeItem().Text(vuelo.NumeroVuelo ?? "N/A");
                                        });
                                        col.Item().Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Origen:").SemiBold();
                                            row.RelativeItem().Text($"{vuelo.AeropuertoOrigen} ({vuelo.CodigoIATAOrigen})");
                                        });
                                        col.Item().Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Destino:").SemiBold();
                                            row.RelativeItem().Text($"{vuelo.AeropuertoDestino} ({vuelo.CodigoIATADestino})");
                                        });
                                        col.Item().Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Fecha Salida:").SemiBold();
                                            row.RelativeItem().Text(vuelo.FechaHoraSalida?.ToString("dd/MM/yyyy HH:mm") ?? "N/A");
                                        });
                                        col.Item().Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Fecha Llegada:").SemiBold();
                                            row.RelativeItem().Text(vuelo.FechaHoraLlegada?.ToString("dd/MM/yyyy HH:mm") ?? "N/A");
                                        });
                                    });
                            });

                            // Información del Asiento
                            column.Item().Element(container =>
                            {
                                container
                                    .Background(Colors.Purple.Lighten5)
                                    .Padding(15)
                                    .Border(1)
                                    .BorderColor(Colors.Purple.Darken1)
                                    .Column(col =>
                                    {
                                        col.Item().Text("INFORMACIÓN DEL ASIENTO").Bold().FontSize(14);
                                        col.Item().PaddingTop(5).Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Número de Asiento:").SemiBold();
                                            row.RelativeItem().Text(boleto.NumeroAsiento ?? "N/A");
                                        });
                                        col.Item().Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Clase:").SemiBold();
                                            row.RelativeItem().Text(boleto.Clase ?? "N/A");
                                        });
                                    });
                            });

                            // Información de Precios
                            column.Item().Element(container =>
                            {
                                container
                                    .Background(Colors.Red.Lighten5)
                                    .Padding(15)
                                    .Border(1)
                                    .BorderColor(Colors.Red.Darken1)
                                    .Column(col =>
                                    {
                                        col.Item().Text("INFORMACIÓN DE PRECIOS").Bold().FontSize(14);
                                        col.Item().PaddingTop(5).Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Precio Base:").SemiBold();
                                            row.RelativeItem().Text($"Q {boleto.Precio:N2}");
                                        });
                                        if (boleto.Descuento.HasValue && boleto.Descuento.Value > 0)
                                        {
                                            col.Item().Row(row =>
                                            {
                                                row.ConstantItem(120).Text("Descuento:").SemiBold();
                                                row.RelativeItem().Text($"Q {boleto.Descuento.Value:N2}");
                                            });
                                        }
                                        if (boleto.Impuesto.HasValue && boleto.Impuesto.Value > 0)
                                        {
                                            col.Item().Row(row =>
                                            {
                                                row.ConstantItem(120).Text("Impuesto:").SemiBold();
                                                row.RelativeItem().Text($"Q {boleto.Impuesto.Value:N2}");
                                            });
                                        }
                                        col.Item().Row(row =>
                                        {
                                            row.ConstantItem(120).Text("TOTAL:").Bold().FontSize(14);
                                            row.RelativeItem().Text($"Q {boleto.Total:N2}").Bold().FontSize(14);
                                        });
                                    });
                            });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.DefaultTextStyle(TextStyle.Default.FontSize(10).FontColor(Colors.Grey.Medium));
                            x.Span("Gracias por elegir nuestra aerolínea. ");
                            x.Span("¡Buen viaje!").Bold();
                        });
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GenerarPdfEmpleado(Models.EmpleadosModel empleado)
        {
            if (empleado == null)
                throw new ArgumentNullException(nameof(empleado));

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .AlignCenter()
                        .Text("👤 PERFIL DE EMPLEADO")
                        .SemiBold()
                        .FontSize(20)
                        .FontColor(Colors.Blue.Darken3);

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(15);

                            // Información Personal
                            column.Item().Element(container =>
                            {
                                container
                                    .Background(Colors.Blue.Lighten5)
                                    .Padding(15)
                                    .Border(1)
                                    .BorderColor(Colors.Blue.Darken1)
                                    .Column(col =>
                                    {
                                        col.Item().Text("INFORMACIÓN PERSONAL").Bold().FontSize(14);
                                        col.Item().PaddingTop(5).Row(row =>
                                        {
                                            row.ConstantItem(120).Text("ID Empleado:").SemiBold();
                                            row.RelativeItem().Text($"#{empleado.IdEmpleado}");
                                        });
                                        col.Item().Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Nombre:").SemiBold();
                                            row.RelativeItem().Text(empleado.Nombre ?? "N/A");
                                        });
                                        col.Item().Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Cargo:").SemiBold();
                                            row.RelativeItem().Text(empleado.Cargo ?? "N/A");
                                        });
                                        col.Item().Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Estado:").SemiBold();
                                            row.RelativeItem().Text(empleado.Estado ?? "N/A");
                                        });
                                    });
                            });

                            // Información de Contacto
                            column.Item().Element(container =>
                            {
                                container
                                    .Background(Colors.Green.Lighten5)
                                    .Padding(15)
                                    .Border(1)
                                    .BorderColor(Colors.Green.Darken1)
                                    .Column(col =>
                                    {
                                        col.Item().Text("INFORMACIÓN DE CONTACTO").Bold().FontSize(14);
                                        col.Item().PaddingTop(5).Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Correo:").SemiBold();
                                            row.RelativeItem().Text(empleado.Correo ?? "N/A");
                                        });
                                        col.Item().Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Teléfono:").SemiBold();
                                            row.RelativeItem().Text(empleado.Telefono.ToString());
                                        });
                                        col.Item().Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Dirección:").SemiBold();
                                            row.RelativeItem().Text(empleado.Direccion ?? "N/A");
                                        });
                                        col.Item().Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Contacto Emergencia:").SemiBold();
                                            row.RelativeItem().Text(empleado.ContactoEmergencia.ToString());
                                        });
                                    });
                            });

                            // Información Profesional
                            column.Item().Element(container =>
                            {
                                container
                                    .Background(Colors.Orange.Lighten5)
                                    .Padding(15)
                                    .Border(1)
                                    .BorderColor(Colors.Orange.Darken1)
                                    .Column(col =>
                                    {
                                        col.Item().Text("INFORMACIÓN PROFESIONAL").Bold().FontSize(14);
                                        col.Item().PaddingTop(5).Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Licencia:").SemiBold();
                                            row.RelativeItem().Text(empleado.Licencia ?? "N/A");
                                        });
                                        col.Item().Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Fecha Ingreso:").SemiBold();
                                            row.RelativeItem().Text(empleado.FechaIngreso.ToString("dd/MM/yyyy"));
                                        });
                                        col.Item().Row(row =>
                                        {
                                            row.ConstantItem(120).Text("ID Usuario:").SemiBold();
                                            row.RelativeItem().Text(empleado.IdUsuario.ToString());
                                        });
                                    });
                            });

                            // Información Financiera
                            column.Item().Element(container =>
                            {
                                container
                                    .Background(Colors.Purple.Lighten5)
                                    .Padding(15)
                                    .Border(1)
                                    .BorderColor(Colors.Purple.Darken1)
                                    .Column(col =>
                                    {
                                        col.Item().Text("INFORMACIÓN FINANCIERA").Bold().FontSize(14);
                                        col.Item().PaddingTop(5).Row(row =>
                                        {
                                            row.ConstantItem(120).Text("Salario:").SemiBold();
                                            row.RelativeItem().Text($"Q {empleado.Salario:N2}").Bold().FontSize(12);
                                        });
                                    });
                            });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.DefaultTextStyle(TextStyle.Default.FontSize(10).FontColor(Colors.Grey.Medium));
                            x.Span("Proyecto Aerolínea - Perfil de Empleado. ");
                            x.Span("Generado el ").FontSize(9);
                            x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(9).Bold();
                        });
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GenerarPdfDashboard(DashboardStats estadisticas, List<VueloResumen> vuelos, string nombreUsuario, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    // Header con saludo
                    page.Header()
                        .Column(column =>
                        {
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Text($"👋 ¡Hola! {nombreUsuario}")
                                    .SemiBold()
                                    .FontSize(18)
                                    .FontColor(Colors.Blue.Darken3);
                            });
                            column.Item().PaddingTop(3).Text("Bienvenido a tu panel de control")
                                .FontSize(10)
                                .FontColor(Colors.Grey.Medium);
                            string rangoFechas = "";
                            if (fechaInicio.HasValue && fechaFin.HasValue)
                            {
                                rangoFechas = $" | Período: {fechaInicio.Value:dd/MM/yyyy} - {fechaFin.Value:dd/MM/yyyy}";
                            }
                            else if (fechaInicio.HasValue)
                            {
                                rangoFechas = $" | Desde: {fechaInicio.Value:dd/MM/yyyy}";
                            }
                            else if (fechaFin.HasValue)
                            {
                                rangoFechas = $" | Hasta: {fechaFin.Value:dd/MM/yyyy}";
                            }
                            column.Item().PaddingTop(2).Text($"📅 Generado el {DateTime.Now:dd/MM/yyyy HH:mm}{rangoFechas}")
                                .FontSize(9)
                                .FontColor(Colors.Grey.Darken1);
                        });

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(12);

                            // Cards de Estadísticas - Diseño igual al dashboard
                            column.Item().Row(row =>
                            {
                                // Card 1: Vuelos Reservados (gradiente púrpura #667eea a #764ba2)
                                row.RelativeItem(1).Element(elem =>
                                {
                                    elem
                                        .Background(Colors.Blue.Darken2) // Púrpura/azul oscuro
                                        .Padding(12)
                                        .Column(c =>
                                        {
                                            c.Item().Row(r =>
                                            {
                                                r.RelativeItem().Column(col =>
                                                {
                                                    col.Item().Text("Vuelos Reservados")
                                                        .FontSize(9)
                                                        .FontColor(Colors.Grey.Lighten3);
                                                    col.Item().PaddingTop(2).Text($"{estadisticas.VuelosReservados}")
                                                        .Bold()
                                                        .FontSize(22)
                                                        .FontColor(Colors.White);
                                                });
                                                r.ConstantItem(30).Text("📅")
                                                    .FontSize(16);
                                            });
                                        });
                                });

                                row.ConstantItem(8); // Espacio entre cards

                                // Card 2: Vuelos Completados (gradiente verde #11998e a #38ef7d)
                                row.RelativeItem(1).Element(elem =>
                                {
                                    elem
                                        .Background(Colors.Green.Darken1) // Verde/turquesa
                                        .Padding(12)
                                        .Column(c =>
                                        {
                                            c.Item().Row(r =>
                                            {
                                                r.RelativeItem().Column(col =>
                                                {
                                                    col.Item().Text("Vuelos Completados")
                                                        .FontSize(9)
                                                        .FontColor(Colors.Grey.Lighten3);
                                                    col.Item().PaddingTop(2).Text($"{estadisticas.VuelosCompletados}")
                                                        .Bold()
                                                        .FontSize(22)
                                                        .FontColor(Colors.White);
                                                });
                                                r.ConstantItem(30).Text("✅")
                                                    .FontSize(16);
                                            });
                                        });
                                });
                            });

                            column.Item().PaddingTop(8).Row(row =>
                            {
                                // Card 3: Vuelos Cancelados (gradiente rojo/naranja #ee0979 a #ff6a00)
                                row.RelativeItem(1).Element(elem =>
                                {
                                    elem
                                        .Background(Colors.Red.Darken1) // Rojo/naranja oscuro
                                        .Padding(12)
                                        .Column(c =>
                                        {
                                            c.Item().Row(r =>
                                            {
                                                r.RelativeItem().Column(col =>
                                                {
                                                    col.Item().Text("Vuelos Cancelados")
                                                        .FontSize(9)
                                                        .FontColor(Colors.Grey.Lighten3);
                                                    col.Item().PaddingTop(2).Text($"{estadisticas.VuelosCancelados}")
                                                        .Bold()
                                                        .FontSize(22)
                                                        .FontColor(Colors.White);
                                                });
                                                r.ConstantItem(30).Text("❌")
                                                    .FontSize(16);
                                            });
                                        });
                                });

                                row.ConstantItem(8); // Espacio entre cards

                                // Card 4: Ingresos Totales (gradiente rosa #f093fb a #f5576c)
                                row.RelativeItem(1).Element(elem =>
                                {
                                    elem
                                        .Background(Colors.Pink.Darken1) // Rosa
                                        .Padding(12)
                                        .Column(c =>
                                        {
                                            c.Item().Row(r =>
                                            {
                                                r.RelativeItem().Column(col =>
                                                {
                                                    col.Item().Text("Ingresos Totales")
                                                        .FontSize(9)
                                                        .FontColor(Colors.Grey.Lighten3);
                                                    col.Item().PaddingTop(2).Text($"${estadisticas.IngresosTotales:N0}")
                                                        .Bold()
                                                        .FontSize(22)
                                                        .FontColor(Colors.White);
                                                });
                                                r.ConstantItem(30).Text("💰")
                                                    .FontSize(16);
                                            });
                                        });
                                });
                            });

                            // Sección de Vuelos Disponibles - Diseño similar al dashboard
                            column.Item().PaddingTop(8).Element(container =>
                            {
                                container
                                    .Background(Colors.White)
                                    .Padding(12)
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Column(col =>
                                    {
                                        col.Item().Row(row =>
                                        {
                                            row.RelativeItem().Text($"✈️ Vuelos Disponibles").Bold().FontSize(14).FontColor(Colors.Blue.Darken3);
                                            row.ConstantItem(100).Element(elem =>
                                            {
                                                elem
                                                    .Background(Colors.Blue.Lighten4)
                                                    .Padding(4)
                                                    .AlignCenter()
                                                    .Text($"{vuelos?.Count ?? 0} Resultado(s)")
                                                    .SemiBold()
                                                    .FontSize(9)
                                                    .FontColor(Colors.Blue.Darken3);
                                            });
                                        });
                                        
                                        col.Item().PaddingTop(10);

                                        if (vuelos != null && vuelos.Any())
                                        {
                                            int contador = 0;
                                            foreach (var vuelo in vuelos.Take(6)) // Máximo 6 vuelos en PDF
                                            {
                                                contador++;
                                                col.Item().PaddingBottom(8).Element(elem =>
                                                {
                                                    elem
                                                        .Background(Colors.Grey.Lighten5)
                                                        .Padding(10)
                                                        .Border(1)
                                                        .BorderColor(Colors.Grey.Lighten1)
                                                        .Column(c =>
                                                        {
                                                            // Header del vuelo
                                                            c.Item().Row(row =>
                                                            {
                                                                row.RelativeItem(2).Text($"{vuelo.NombreAerolinea}").SemiBold().FontSize(11).FontColor(Colors.Blue.Darken3);
                                                                row.RelativeItem(1).Text($"${vuelo.PrecioMinimo:N0}").Bold().FontSize(12).FontColor(Colors.Blue.Darken2).AlignRight();
                                                            });
                                                            
                                                            // Información de ruta y horario
                                                            c.Item().PaddingTop(6).Row(row =>
                                                            {
                                                                row.RelativeItem(1).Column(colInfo =>
                                                                {
                                                                    colInfo.Item().Text("Origen").FontSize(8).FontColor(Colors.Grey.Darken1);
                                                                    colInfo.Item().PaddingTop(2).Text(vuelo.AeropuertoOrigen).SemiBold().FontSize(9);
                                                                    colInfo.Item().PaddingTop(2).Text($"{vuelo.HoraSalida:hh\\:mm}").FontSize(10);
                                                                });
                                                                row.RelativeItem(2).Column(colInfo =>
                                                                {
                                                                    colInfo.Item().Text("Duración").FontSize(8).FontColor(Colors.Grey.Darken1).AlignCenter();
                                                                    var duracion = vuelo.HoraLlegada - vuelo.HoraSalida;
                                                                    colInfo.Item().PaddingTop(2).Text($"{duracion.Hours}h {duracion.Minutes}m").FontSize(9).AlignCenter();
                                                                    colInfo.Item().PaddingTop(2).Text($"Sin Escalas").FontSize(8).FontColor(Colors.Grey.Medium).AlignCenter();
                                                                    colInfo.Item().PaddingTop(2).Text(vuelo.FechaVuelo.ToString("dd MMM yyyy")).FontSize(8).FontColor(Colors.Grey.Darken1).AlignCenter();
                                                                });
                                                                row.RelativeItem(1).Column(colInfo =>
                                                                {
                                                                    colInfo.Item().Text("Destino").FontSize(8).FontColor(Colors.Grey.Darken1).AlignRight();
                                                                    colInfo.Item().PaddingTop(2).Text(vuelo.AeropuertoDestino).SemiBold().FontSize(9).AlignRight();
                                                                    colInfo.Item().PaddingTop(2).Text($"{vuelo.HoraLlegada:hh\\:mm}").FontSize(10).AlignRight();
                                                                });
                                                            });
                                                        });
                                                });
                                            }

                                            if (vuelos.Count > 6)
                                            {
                                                col.Item().PaddingTop(5).Text($"... y {vuelos.Count - 6} vuelo(s) más")
                                                    .Italic()
                                                    .FontSize(9)
                                                    .FontColor(Colors.Grey.Medium)
                                                    .AlignCenter();
                                            }
                                        }
                                        else
                                        {
                                            col.Item().PaddingVertical(20).AlignCenter().Text("No hay vuelos disponibles en este momento.")
                                                .FontSize(11)
                                                .FontColor(Colors.Grey.Darken1);
                                        }
                                    });
                            });

                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.DefaultTextStyle(TextStyle.Default.FontSize(9).FontColor(Colors.Grey.Medium));
                            x.Span("Proyecto Aerolínea - Dashboard Report. ");
                            x.Span("Página ").FontSize(8);
                            x.CurrentPageNumber().FontSize(8);
                            x.Span(" / ").FontSize(8);
                            x.TotalPages().FontSize(8);
                        });
                });
            });

            return document.GeneratePdf();
        }
    }
}

