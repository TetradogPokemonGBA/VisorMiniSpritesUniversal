/*
 * Creado por SharpDevelop.
 * Usuario: tetra
 * Fecha: 25/05/2017
 * Hora: 0:19
 * Licencia GNU GPL V3
 * Para cambiar esta plantilla use Herramientas | Opciones | Codificación | Editar Encabezados Estándar
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Gabriel.Cat.Wpf;
using Microsoft.Win32;
using PokemonGBAFrameWork;
namespace MinisPaletaDiscover
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public static readonly Bitmap Bmp1,Bmp2,Bmp3;
		IList<MiniSprite> minis;
		PaletasMinis paletas;
		
		static Window1()
		{
			ColorTable ct=new ColorTable();
			
			Bmp1=ct.ColorPicker.Imagen1.ToGbaBitmap();
			Bmp2=ct.ColorPicker.Imagen2.ToGbaBitmap();
			Bmp3=ct.ColorPicker.Imagen3.ToGbaBitmap();
		}
		public Window1()
		{
			
			InitializeComponent();
			CargarRom();
		}

		void CargarRom()
		{
			OpenFileDialog opn=new OpenFileDialog();
			RomGba rom;
			EdicionPokemon edicion;
			Compilacion compilacion;
			DateTime inicio=new DateTime();
			opn.Filter="PokemonGBA|*.gba";
			if(opn.ShowDialog().GetValueOrDefault())
			{
				Title="Cargando";
				rom=new RomGba(opn.FileName);
				
				edicion=EdicionPokemon.GetEdicionPokemon(rom);
				compilacion=Compilacion.GetCompilacion(rom,edicion);
				if(System.Diagnostics.Debugger.IsAttached)
					inicio=DateTime.Now;
				paletas=PaletasMinis.GetPaletasMinis(rom,edicion,compilacion);
				
				minis=MiniSprite.GetMiniSprites(rom,edicion,compilacion,paletas);
				if(System.Diagnostics.Debugger.IsAttached)
					MessageBox.Show((DateTime.Now-inicio).TotalSeconds+"");
				stkMinis.Children.Clear();
				stkPaletas.Children.Clear();
				
				for(int i=0;i<paletas.Paletas.Count;i++)
					AddPaleta(paletas.Paletas[i]);
				for(int i=0;i<minis.Count;i++)
					stkMinis.Children.Add(new MiniViwer(minis[i]));
				Title="Minis Paleta Discover";
			}
			else if(minis!=null)
			{
				MessageBox.Show("No se ha cambiado la rom");
			}
			else{
				MessageBox.Show("No hay rom cargada...");
			}
		}

		void AddPaleta(Paleta paleta)
		{
			Border border=new Border();
			ColorTable ctPaleta=new ColorTable((System.Drawing.Color[])paleta.Colores.Clone());
			ctPaleta.ColorPicker.Imagen1=Bmp1;
			ctPaleta.ColorPicker.Imagen2=Bmp2;
			ctPaleta.ColorPicker.Imagen3=Bmp3;
			
			ctPaleta.ColorChanged+=CambiarPaletaMinis;
			ctPaleta.MouseLeftButtonUp+=PonPaletaATodos;
			ctPaleta.Tag=paleta;
			border.Child=ctPaleta;
			border.BorderThickness=new Thickness(10);
			stkPaletas.Children.Add(border);
			
		}

		void PonPaletaATodos(object sender, MouseButtonEventArgs e)
		{
			System.Drawing.Color[] paleta=((ColorTable)sender).Colors;
			Title="Cargando";
			for(int i=0;i<stkMinis.Children.Count;i++)
				((MiniViwer)stkMinis.Children[i]).SetPaleta(paleta);
			Title="Minis Paleta Discover";
		}
		void CambiarPaletaMinis(object sender, ColorChangedArgs e)
		{
			//los que la tengan se la ponen...
			MiniViwer miniViwer;
			ColorTable ct=(ColorTable)sender;
			Paleta paletaOri=ct.Tag as Paleta;
			System.Drawing.Color[] paleta=ct.Colors;
			Title="Cargando";
			for(int i=0;i<stkMinis.Children.Count;i++)
			{
				miniViwer=(MiniViwer) stkMinis.Children[i];
				if(miniViwer.TienePaleta(paletaOri.Colores))
					miniViwer.SetPaleta(paleta);
			}
			Title="Minis Paleta Discover";
		}
		void BtnRestaurarPaleta_Click(object sender, RoutedEventArgs e)
		{
			Title="Cargando";
			for(int i=0;i<stkMinis.Children.Count;i++)
				((MiniViwer)stkMinis.Children[i]).RestaurarPaleta();
			Title="Minis Paleta Discover";
		}
		void BtnRestaurarPaletas_Click(object sender, RoutedEventArgs e)
		{
			Title="Cargando";
			for(int i=0;i<paletas.Paletas.Count;i++)
				(((Border)stkPaletas.Children[i]).Child as ColorTable).Colors=paletas.Paletas[i].Colores;
			Title="Minis Paleta Discover";
		}
		void MiCargar_Click(object sender, RoutedEventArgs e)
		{
			CargarRom();
		}
		void MiSobre_Click(object sender, RoutedEventArgs e)
		{
			if(MessageBox.Show("Autor:pikachu240\nLicencia:GNU GPL V3\nHecho para Sangus103\n¿Quieres ver el código fuente?","Sobre la app",MessageBoxButton.YesNo,MessageBoxImage.Information)==MessageBoxResult.Yes)
				System.Diagnostics.Process.Start("https://github.com/TetradogPokemonGBA/VisorMiniSpritesUniversal");
		}
	}
}