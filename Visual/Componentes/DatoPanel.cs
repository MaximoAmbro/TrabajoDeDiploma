namespace Visual.Componentes
{
    public partial class DatoPanel : SuperficieTematica
    {
        public DatoPanel(){InitializeComponent();valor.Font=Tema.PrecioTarjeta;valor.ForeColor=Tema.Primario;Paint+=Disposicion.PintarTarjeta;}
        public void Cargar(string nombre,string dato){titulo.Text=nombre;valor.Text=dato;AccessibleName=nombre+": "+dato;}
    }
}
