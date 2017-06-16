/*
 * Creado por SharpDevelop.
 * Usuario: tetra
 * Fecha: 25/05/2017
 * Hora: 0:23
 * Licencia GNU GPL V3
 * Para cambiar esta plantilla use Herramientas | Opciones | Codificación | Editar Encabezados Estándar
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Gabriel.Cat;
using Gabriel.Cat.Extension;
using Microsoft.Win32;
using PokemonGBAFrameWork;
namespace MinisPaletaDiscover
{
	/// <summary>
	/// Interaction logic for MiniViwer.xaml
	/// </summary>
	public partial class MiniViwer : UserControl
	{
		static bool isLight=true;
		public const string EXTENSIONPALETA=".pms";
		MiniSprite mini;
		public MiniViwer(MiniSprite mini)
		{
			if(mini==null)
				throw new ArgumentNullException("mini");
			
			ContextMenu menu=new ContextMenu();
			MenuItem item=new MenuItem();
			Image imgMini;
			
			this.mini=mini;
			
			item.Header="Guardar Paleta";
			item.Click+=GuardarPaleta;
			menu.Items.Add(item);
			
			item=new MenuItem();
			item.Header="Cargar Paleta";
			item.Click+=CargarPaleta;
			menu.Items.Add(item);
			
			item=new MenuItem();
			item.Header="Restaurar Paleta";
			item.Click+=RestaurarPaleta;
			menu.Items.Add(item);
			
			
			
			InitializeComponent();
			ContextMenu=menu;
			ctPaleta.ColorPicker.Imagen1=Window1.Bmp1;
			ctPaleta.ColorPicker.Imagen2=Window1.Bmp2;
			ctPaleta.ColorPicker.Imagen3=Window1.Bmp3;
			ctPaleta.Colors=new System.Drawing.Color[Paleta.LENGTH];
			
			for(int i=0;i<mini.Minis.Count;i++)
			{
				imgMini=new Image();
				menu=new ContextMenu();
				item=new MenuItem();
				item.Header="Guardar Mini";
				item.Click+=GuardarMini;
				item.Tag=imgMini;
				menu.Items.Add(item);
				
				item=new MenuItem();
				item.Header="Copy offset";
				item.Click+=(s,e)=>CopiarOffset(s as MenuItem);
				item.Tag=i;
				menu.Items.Add(item);
				
				imgMini.ContextMenu=menu;
				imgMini.SetImage(mini.Minis[i].GetBitmap(mini.Paleta));
				ugMinis.Children.Add(imgMini);
			}
			
			RestaurarPaleta();
			if(!isLight)
			{
				Background=Brushes.LightBlue;
				
			}else{
				Background=Brushes.White;
			}
			isLight=!isLight;
		}

		void CopiarOffset(MenuItem miMini)
		{
			Clipboard.SetText((Hex)mini.Minis[(int)miMini.Tag].Offset);
		}
		void GuardarMini(object sender, RoutedEventArgs e)
		{
			Image img=(sender as MenuItem).Tag as Image;
			SaveFileDialog sfdImgMini=new SaveFileDialog();

			if(img!=null&&sfdImgMini.ShowDialog().GetValueOrDefault())
			{
				try{
					img.ToBitmap().Save(sfdImgMini.FileName+".png",System.Drawing.Imaging.ImageFormat.Png);
				}catch{
					MessageBox.Show("Ha ocurrido un problema al exportar la imagen del mini","No se ha podido hacer...",MessageBoxButton.OK,MessageBoxImage.Error);
				}
			}
		}

		public bool TienePaleta(System.Drawing.Color[] paleta)
		{
			bool tienePaleta=true;
			for(int i=0;i<paleta.Length&&tienePaleta;i++)
				tienePaleta=paleta[i].Equals(mini.Paleta.Colores[i]);
			
			return tienePaleta;
		}

		public void RestaurarPaleta(object sender=null, RoutedEventArgs e=null)
		{
			SetPaleta(mini.Paleta.Colores);
		}
		public void SetPaleta(System.Drawing.Color[] paleta)
		{
			for(int i=0;i<paleta.Length;i++)
				ctPaleta.Colors[i]=paleta[i];
			ctPaleta.Colors=ctPaleta.Colors;
			CambioPaletaMinis();
		}
		void CargarPaleta(object sender=null, RoutedEventArgs e=null)
		{
			const int BYTESCOLOR=4;
			OpenFileDialog opn=new OpenFileDialog();
			BinaryReader br=null;
			opn.Filter="PaletaMiniSprite|*."+EXTENSIONPALETA+"|TODOS|*.*";
			if(opn.ShowDialog().GetValueOrDefault())
			{
				try{
					br=new BinaryReader(new FileStream(opn.FileName,FileMode.Open));
					for(int i=0;i<ctPaleta.Colors.Length;i++)
						ctPaleta.Colors[i]=Serializar.ToColor(br.ReadBytes(BYTESCOLOR));
				}
				finally{
					br.Close();
					
				}
				CambioPaletaMinis();
			}
		}
		void GuardarPaleta(object sender, RoutedEventArgs e)
		{
			SaveFileDialog sfdPaleta=new SaveFileDialog();
			BinaryWriter bw;
			if(sfdPaleta.ShowDialog().GetValueOrDefault())
			{
				if(File.Exists(sfdPaleta.FileName))
					File.Delete(sfdPaleta.FileName);
				
				bw=new BinaryWriter(new FileStream(sfdPaleta.FileName+EXTENSIONPALETA,FileMode.Create));
				for(int i=0;i<ctPaleta.Colors.Length;i++)
					bw.Write(Serializar.GetBytes(ctPaleta.Colors[i]));
				bw.Close();
				
				
			}
		}

		void CtPaleta_ColorChanged(object sender, Gabriel.Cat.Wpf.ColorChangedArgs e)
		{
			CambioPaletaMinis();
		}
		void CambioPaletaMinis()
		{
			Paleta paleta=new Paleta(ctPaleta.Colors);
			//actualizo los minis
			for(int i=0;i<mini.Minis.Count;i++)
				((Image)ugMinis.Children[i]).SetImage(mini.Minis[i].GetBitmap(paleta));
			
		}
	}
}