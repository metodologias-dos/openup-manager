namespace OpenUpMan.Domain
{
    public class PhaseArtefact
    {
        public Guid PhaseId { get; private set; }
        public ProjectPhase? Phase { get; private set; }
        public Guid ArtefactId { get; private set; }
        public Artefact? Artefact { get; private set; }
        public Guid? DocumentId { get; private set; }
        public Document? Document { get; private set; }
        public bool Registrado { get; private set; }  // 0 = no, 1 = sí

        // Parameterless constructor for EF
        protected PhaseArtefact() { }

        public PhaseArtefact(Guid phaseId, Guid artefactId, Guid? documentId = null, bool registrado = false)
        {
            if (phaseId == Guid.Empty)
            {
                throw new ArgumentException("PhaseId cannot be an empty GUID.", nameof(phaseId));
            }

            if (artefactId == Guid.Empty)
            {
                throw new ArgumentException("ArtefactId cannot be an empty GUID.", nameof(artefactId));
            }

            PhaseId = phaseId;
            ArtefactId = artefactId;
            DocumentId = documentId;
            Registrado = registrado;
        }

        public void SetDocument(Guid? documentId)
        {
            DocumentId = documentId;
        }

        public void MarkAsRegistered()
        {
            Registrado = true;
        }

        public void MarkAsUnregistered()
        {
            Registrado = false;
        }

        public void SetRegistrado(bool registrado)
        {
            Registrado = registrado;
        }
    }
}

