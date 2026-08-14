document.addEventListener("DOMContentLoaded", function () {

    const cards = document.querySelectorAll(".project-card");
    const columns = document.querySelectorAll(".kanban-column");

    console.log("Kanban carregado!");
    console.log("Cartões encontrados:", cards.length);
    console.log("Colunas encontradas:", columns.length);

    let draggedCard = null;

    cards.forEach(function (card) {

        card.addEventListener("dragstart", function (event) {

            console.log(
                "Começou a arrastar:",
                card.dataset.projectId
            );

            draggedCard = card;

            card.classList.add("dragging");

            event.dataTransfer.effectAllowed = "move";

            event.dataTransfer.setData(
                "text/plain",
                card.dataset.projectId
            );

        });

        card.addEventListener("dragend", function () {

            console.log("Terminou de arrastar");

            card.classList.remove("dragging");

            draggedCard = null;

        });

    });


    columns.forEach(function (column) {

        column.addEventListener("dragover", function (event) {

            event.preventDefault();

        });


        column.addEventListener("drop", async function (event) {

            event.preventDefault();

            console.log("DROP!");

            if (!draggedCard) {

                console.log(
                    "Nenhum cartão sendo arrastado."
                );

                return;
            }

            // Guarda o cartão antes do fetch
            // para ele não virar null durante o dragend
            const cardToMove = draggedCard;

            const projectId =
                cardToMove.dataset.projectId;

            const newStatus =
                column.dataset.status;


            console.log("Projeto:", projectId);
            console.log("Novo status:", newStatus);


            try {

                console.log("1 - Enviando requisição...");

                const response = await fetch(
                    "/Project/UpdateStatusAjax",
                    {
                        method: "POST",

                        headers: {
                            "Content-Type": "application/json"
                        },

                        body: JSON.stringify({
                            projectId: parseInt(projectId),
                            status: newStatus
                        })
                    }
                );


                console.log("2 - Fetch terminou");
                console.log("3 - Status:", response.status);
                console.log("4 - OK:", response.ok);


                if (!response.ok) {

                    console.error(
                        "Erro HTTP:",
                        response.status
                    );

                    alert(
                        "Não foi possível atualizar o status."
                    );

                    location.reload();

                    return;
                }


                console.log(
                    "5 - Antes de mover cartão"
                );

                console.log(
                    "Card:",
                    cardToMove
                );

                console.log(
                    "Column:",
                    column
                );


                // Move o cartão imediatamente
                column.appendChild(cardToMove);


                console.log(
                    "6 - Cartão movido!"
                );


                console.log(
                    "Status atualizado com sucesso!"
                );


                // Recarrega para atualizar
                // as contagens das colunas
                setTimeout(function () {

                    location.reload();

                }, 300);


            } catch (error) {

                console.error(
                    "ERRO COMPLETO:",
                    error
                );

                console.error(
                    "Nome:",
                    error.name
                );

                console.error(
                    "Mensagem:",
                    error.message
                );

                console.error(
                    "Stack:",
                    error.stack
                );

                alert(
                    "Não foi possível conectar ao servidor."
                );

            }

        });

    });

});

function openProjectDetails(event, projectId) {

    // Não abre os detalhes se o usuário estava arrastando
    if (event.defaultPrevented) {
        return;
    }

    window.location.href = `/Project/Details/${projectId}`;
}