using OnePlace.Client.ComponentesGenericos.Base;
using OnePlace.Client.ComponentesGenericos.Utilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlace.Client.ComponentesGenericos.ModalContent
{
	public partial class ModalContent : SimComponentBase
	{
		//estas clases las puse para que pueda cambiar de tamño el modal en base a boostrap con las clases modal-lg, modal-sm
		protected string ModalClass => new CssBuilder("modal bd-example-modal-lg")
		 .AddClass(ClassModal)
	   .Build();
		protected string ModalDialogClass => new CssBuilder("modal-dialog modal-lg")
		  .AddClass(ClassModalDialog)
		.Build();

		//esto se puso para darle un estilo predefinido o puede otorgar uno diferente
		protected string Stylename =>
			new StyleBuilder()
			.AddStyle("display", "block")
			.AddStyle("background-color", "rgba(10,10,10,.8)")
			.AddStyle(Style)
			.Build();
		[Parameter] public string ClassModal { get; set; }
		[Parameter] public string ClassModalDialog { get; set; }


		[Parameter] public bool EsVisible { get; set; }
		[Parameter] public string Titulo { get; set; }
		[Parameter] public RenderFragment ChildContent { get; set; }
		[Parameter] public RenderFragment ModalCabecera { get; set; }
		[Parameter] public RenderFragment ModalPie { get; set; }
		[Parameter] public string TituloEnlace { get; set; }
		[Parameter] public string Href { get; set; }
		[Parameter] public EventCallback MetodoOnclick { get; set; }
		[Parameter] public EventCallback OnValidSubmit { get; set; }
		[Parameter] public EventCallback<bool> OnClose { get; set; }
		[Parameter] public TiposdeBoton BotonesModal { get; set; }
		public enum TiposdeBoton
		{
			Ok,
			OkCancelar,
			CancelarBorrar,
			Cancelar,
			Link,
			SinBtn,
			LinkOK,
			BotonOnclick
		}
		private Task ModalCancel()
		{
			return OnClose.InvokeAsync(false);
		}

		//tipo submit
		private Task ModalOk()
		{
			//return OnClose.InvokeAsync(true);
			return OnValidSubmit.InvokeAsync();
		}
		//tipo button 
		private Task ModalOk2()
		{
			return MetodoOnclick.InvokeAsync();
		}
	}
}
