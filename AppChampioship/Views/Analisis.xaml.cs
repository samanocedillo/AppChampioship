﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AppChampioship.Services;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppChampioship.Views
{


        
		[XamlCompilation(XamlCompilationOptions.Compile)]
		public partial class Analisis : ContentPage
		{


            string id="", nombre="", email="", emotion="";

			static System.IO.Stream streamCopy;




			public Analisis()
			{
				InitializeComponent();
			}

        async void Handle_CLlicked(object sender, System.EventArgs e)
        {
           //await Authenticate();
        }



        async void btnFoto_Clicked(object sender, EventArgs e)
			{
			var usarCamara = ((Button)sender).Text.Contains("cámara");
			var file = await ServicioImagenes.TomarFoto(usarCamara);
			panelResultados.Children.Clear();
			lblResultado.Text = "---";

				imgFoto.Source = ImageSource.FromStream(() =>
				{
					var stream = file.GetStream();
					streamCopy = new MemoryStream();
					stream.CopyTo(streamCopy);
					stream.Seek(0, SeekOrigin.Begin);
					file.Dispose();

					return stream;
				});
			}


			async void btnAnalizarEmociones_Clicked(object sender, EventArgs e)
			{
			    if (streamCopy != null)
			    {
			    	streamCopy.Seek(0, SeekOrigin.Begin);
			    	var emociones = await
			    	ServicioEmociones.ObtenerEmociones(streamCopy);
			    	if (emociones != null)
			    	{

                        this.id = "app" + 1;
                        this.nombre = txtNombre.Text.ToString();
                        this.email = txtCorreo.Text.ToString();
                        
                    foreach (var item in this.emotion)
                    {
                        this.emotion = emociones.ToString();
                    }

                    lblResultado.Text = " ---Análisis de Emociones---";
			    		DibujarResultados(emociones);
			    	}
			    	else lblResultado.Text = " ---No se detectó una cara---";
			    }
			    else lblResultado.Text = " ---No has seleccionado una imagen---";

			}

			async void DibujarResultados(Dictionary<string, float> emociones)
			{

			try
			{
				ServiceHelper serviceHelper = new ServiceHelper();
				// Retrieve the values the user entered into the UI
				if (string.IsNullOrEmpty(nombre) && string.IsNullOrEmpty(email))
				{
                    await DisplayAlert("Mensaje", "Por favor introduce un correo electronico valido", "OK");
				}
				else
				{
					await serviceHelper.InsertarEntidad(id, nombre, email, emotion);

                    await DisplayAlert("Mensaje", "Gracias por registrarte", "OK");
					//SetResult(Result.Ok, Intent);
				}

			}
			catch (Exception e)
			{
				await DisplayAlert("Mensaje", ""+e+"", "OK" );
			}


				panelResultados.Children.Clear();
				foreach (var emocion in emociones)
				{
					Label lblEmocion = new Label()
					{
						Text = emocion.Key,
						TextColor = Color.Blue,
						WidthRequest = 90
					};

					BoxView box = new BoxView()
					{
						Color = Color.Lime,
						WidthRequest = 150 * emocion.Value,
						HeightRequest = 30,
						HorizontalOptions = LayoutOptions.StartAndExpand
					};

					Label lblPorcentaje = new Label()
					{
						Text = emocion.Value.ToString("P4"),
						TextColor = Color.Maroon
					};

					StackLayout panel = new StackLayout()
					{
						Orientation = StackOrientation.Horizontal
					};

					panel.Children.Add(lblEmocion);
					panel.Children.Add(box);
					panel.Children.Add(lblPorcentaje);
					panelResultados.Children.Add(panel);
				}

			}

        private void SetResult(object ok, object intent)
        {
            throw new NotImplementedException();
        }


    }

}
