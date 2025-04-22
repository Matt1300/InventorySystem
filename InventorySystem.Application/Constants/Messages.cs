namespace InventorySystem.Application.Constants
{
    public static class Messages
    {
        public const string ProductsRetrieved = "Productos obtenidos correctamente";
        public const string BatchesRetrieved = "Lotes obtenidos correctamente";
        public const string EmptyRequest = "Faltan datos o está vacío algún campo";
        public const string ProductAdded = "Producto agregado con éxito";
        public const string ProductUpdated = "Producto actualizado con éxito";
        public const string BatchUpdated  = "Lote actualizado con éxito";
        public const string ProductNotFound = "Producto no encontrado";
        public const string BatchNotFound = "Lote no encontrado";
        public const string ErrorOccurred = "Ocurrió un error inesperado";
        public const string ProductDeleted = "Producto Eliminado";
        public const string BatchDeleted  = "Lote Eliminado";
        public const string ProductStatusModifier = "Este producto se marcó como inactivo porque ya tenía registros anteriores.";
        public const string UserRegistered = "Usuario registrado con éxito";
        public const string UserNotFound = "Usuario no encontrado";
        public const string SuccessLogin = "Login Exitoso";
        public const string LoginFailed = "Credenciales incorrectas, intentelo de nuevo";
        public const string UserAlreadyExists  = "Este usuario ya ha sido registrado, intente con un nuevo";
        public const string InvalidToken = "Refresh token o usuario invalido";
        public const string ValidToken = "Nuevo refresh token generado";
        public const string BatchAlreadyExists = "Ya existe un lote con ese código";
        public const string BatchAdded = "Nuevo lote agreagado";
        public const string LowStockBatchesRetrieved = "Cantidad de productos con bajo stock obtenido";
    }
}
