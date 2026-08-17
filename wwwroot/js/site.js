document.addEventListener("DOMContentLoaded", function () {

    const cards = document.querySelectorAll(".project-card");
    const columns = document.querySelectorAll(".kanban-column");

    console.log("Kanban carregado!");
    console.log("Cartões encontrados:", cards.length);
    console.log("Colunas encontradas:", columns.length);

    let draggedCard = null;


    // ==========================================
    // INÍCIO DO DRAG
    // ==========================================

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


        // ==========================================
        // FIM DO DRAG
        // ==========================================

        card.addEventListener("dragend", function () {

            console.log("Terminou de arrastar");

            card.classList.remove("dragging");

            draggedCard = null;

        });

    });


    // ==========================================
    // COLUNAS
    // ==========================================

    columns.forEach(function (column) {

        column.addEventListener("dragover", function (event) {

            event.preventDefault();

            event.dataTransfer.dropEffect = "move";

        });


        // ==========================================
        // DROP
        // ==========================================

        column.addEventListener("drop", async function (event) {

            event.preventDefault();

            console.log("DROP!");


            if (!draggedCard) {

                console.log(
                    "Nenhum cartão sendo arrastado."
                );

                return;
            }


            const cardToMove = draggedCard;

            const projectId =
                cardToMove.dataset.projectId;

            const newStatus =
                column.dataset.status;


            console.log("Projeto:", projectId);
            console.log("Novo status:", newStatus);


            // Guarda a coluna original
            const originalColumn =
                cardToMove.parentElement;


            // Não faz requisição se soltou
            // na mesma coluna
            if (originalColumn === column) {

                console.log(
                    "O projeto já está nesta coluna."
                );

                return;
            }


            try {

                console.log(
                    "Enviando atualização para o servidor..."
                );


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


                console.log(
                    "Resposta:",
                    response.status
                );


                // ==========================================
                // ERRO
                // ==========================================

                if (!response.ok) {

                    const errorData =
                        await response.json()
                            .catch(() => null);

                    console.error(
                        "Erro ao atualizar:",
                        errorData
                    );


                    alert(
                        errorData?.message ??
                        "Não foi possível atualizar o status."
                    );


                    return;
                }


                // ==========================================
                // SUCESSO
                // ==========================================

                const data =
                    await response.json();


                console.log(
                    "Servidor:",
                    data
                );


                // Move o card
                column.appendChild(cardToMove);


                console.log(
                    "Cartão movido com sucesso!"
                );


                // Atualiza a contagem das colunas
                updateColumnCounts();


            } catch (error) {

                console.error(
                    "Erro completo:",
                    error
                );


                alert(
                    "Não foi possível conectar ao servidor."
                );

            }

        });

    });


    // ==========================================
    // CONTAGEM DAS COLUNAS
    // ==========================================

    function updateColumnCounts() {

        columns.forEach(function (column) {

            const count =
                column.querySelectorAll(".project-card").length;

            const counter =
                column.querySelector(".column-count");


            if (counter) {
                counter.textContent = count;
            }

        });

    }

});


// ==========================================
// ABRIR DETALHES DO PROJETO
// ==========================================

function openProjectDetails(event, projectId) {

    if (event.defaultPrevented) {
        return;
    }

    window.location.href =
        `/Project/Details/${projectId}`;
}